import os.path
from urllib.request import urlretrieve
from urllib import error
from tempfile import TemporaryDirectory
from zipfile import ZipFile
from argparse import ArgumentParser
import subprocess
import shutil
import fileinput


def download_file(url, file_path):
    """
    Downloads a file from 'url' to the 'file_path'.
    """
    try:
        urlretrieve(url, file_path)
    except error.URLError as url_error:
        print(f"Error during file downloading. Reason: {url_error.reason}")
        exit(1)


##############
# Get command line args
##############
args_parser = ArgumentParser(
    prog="setup",
    description="setup script for SampleGame2048 game"
)
args_parser.add_argument("-u", "--unity", metavar="UNITY_EXE_PATH", help="Path to the Unity executable", required=True)
args = args_parser.parse_args()

##############
# Download ubuntu-sans font
##############
print("Downloading Ubuntu Sans font...", end="")
download_file(
    "https://github.com/canonical/Ubuntu-Sans-fonts/raw/refs/heads/main/fonts/ttf/UbuntuSans-Bold.ttf",
    "./Assets/Fonts/UbuntuSans/UbuntuSans-Bold.ttf"
)
print("OK")

##############
# Restore NuGet packages
##############
# check if dotnet is installed
if shutil.which("dotnet") is None:
    print("'dotnet' doesn't exist, can't continue the installation")
    exit(1)
# check if NuGetForUnity.Cli tool is already installed globally
global_tool_list = subprocess.run("dotnet tool list -g", check=True, stdout=subprocess.PIPE).stdout.decode("utf-8")
nuget_tool_installed_locally = False
if "nugetforunity" not in global_tool_list:
    # install tool locally
    print("Installing NuGetForUnity.Cli as a local tool...", end="")
    subprocess.run("dotnet new tool-manifest --force", check=True, stdout=subprocess.DEVNULL)
    subprocess.run("dotnet tool install --local NuGetForUnity.Cli", check=True, stdout=subprocess.DEVNULL)
    print("OK")
    nuget_tool_installed_locally = True
# run tool (restore NuGet packages)
print("Restoring NuGet packages...", end="")
subprocess.run('dotnet nugetforunity restore "./"', check=True, stdout=subprocess.DEVNULL)
print("OK")
# uninstall tool if it was installed locally
if nuget_tool_installed_locally:
    print("Uninstalling NuGetForUnity.Cli local tool...", end="")
    subprocess.run("dotnet tool uninstall NuGetForUnity.Cli", check=True, stdout=subprocess.DEVNULL)
    # also remove .config/ directory
    shutil.rmtree("./.config")
    print("OK")

with TemporaryDirectory() as temp_directory:
    ##############
    # Download and install DOTween library
    ##############
    dotween_archive_path = f"{temp_directory}/dotween.zip"
    print("Downloading DOTween lib...", end="")
    download_file(
        "https://dotween.demigiant.com/downloads/DOTween_1_2_765.zip",
        dotween_archive_path
    )
    print("OK")
    # extract DOTween library
    print("Extracting DOTween lib...", end="")
    plugins_directory_path = "./Assets/Plugins/"
    with ZipFile(dotween_archive_path, "r") as dotween_zip:
        dotween_zip.extractall(path=plugins_directory_path)
    print("OK")

    ##############
    # Download Extenject package
    ##############
    extenject_package_path = f"{temp_directory}/Extenject.unitypackage"
    print("Downloading Extenject package...", end="")
    download_file(
        "https://github.com/Mathijs-Bakker/Extenject/releases/download/9.2.0/Extenject.unitypackage",
        extenject_package_path
    )
    print("OK")

    ##############
    # Move Src folder away
    ##############
    # Src folder should not be in the Assets folder during the unity package installation in batchmode, otherwise
    # there will be compiler errors preventing the package to be installed (despite the various 'ignore errors' settings).
    # Also move the .meta file, otherwise empty Src folder will be created during the package installation
    print("Moving Src folder to the temporary directory...", end="")
    shutil.move("./Assets/Src", f"{temp_directory}/Src")
    shutil.move("./Assets/Src.meta", f"{temp_directory}/Src.meta")
    print("OK")

    ##############
    # Install Extenject package using unity
    ##############
    unity_log_path = "./log.txt"
    extenject_package_installed = False
    print(
        "Initializing the project and installing Extenject package (it can take up to several minutes)...",
        flush=True,
        end=""
    )
    try:
        unity_process_args = [
            args.unity,
            "-projectPath",
            "./",
            "-batchmode",
            "-nographics",
            "-silent-crashes",
            "‑ignorecompilererrors",
            "-importPackage",
            extenject_package_path,
            "-logFile",
            unity_log_path,
            "-quit"
        ]
        subprocess.run(args=unity_process_args, check=True, stdout=subprocess.DEVNULL)
        extenject_package_installed = True
    except subprocess.CalledProcessError as unity_process_error:
        print(f"\nError during package installation, see {unity_log_path} for details")

    if extenject_package_installed:
        print("OK")

    ##############
    # Move Src folder back
    ##############
    print("Moving Src folder back to the Assets folder...", end="")
    shutil.move(f"{temp_directory}/Src", "./Assets/Src")
    shutil.move(f"{temp_directory}/Src.meta", "./Assets/Src.meta")
    print("OK")

    # exit here if there were some issues during the package installation, because it was necessary to move
    # the Src folder back before deleting the temporary folder
    if not extenject_package_installed:
        exit(1)

##############
# Remove unused OptionalExtras from the Extenject
##############
print("Removing unused Extenject OptionalExtras...", end="")
optional_extras_path = "./Assets/Plugins/Zenject/OptionalExtras/"
excluded_optional_extras = ["Signals", "Signals.meta"]
for item in [item for item in os.listdir(optional_extras_path)
             if item not in excluded_optional_extras]:
    item_path = optional_extras_path + item
    if os.path.isfile(item_path):
        os.remove(item_path)
    else:
        # remove directory
        shutil.rmtree(item_path)
# also remove unnecessary project files for deleted OptionalExtras
project_files_path = "./"
excluded_project_files = ["Zenject.csproj", "Zenject-Editor.csproj"]
for item in [item for item in os.listdir(project_files_path)
             if item.startswith("Zenject")
             and item not in excluded_project_files]:
    os.remove(project_files_path + item)
print("OK")

##############
# Patch Extenject for R3 support
##############
# Extenject currently supports only R3's predecessor (UniRx), so it needs to be patched in order to use the R3
files_to_patch = [
    f"{optional_extras_path}Signals/Internal/SignalDeclaration.cs",
    f"{optional_extras_path}Signals/Main/SignalBus.cs"
]
patch_replacements = {
    "UNIRX": "R3",
    "UniRx": "R3",
    "IObservable": "Observable",
    "&& !_stream.HasObservers": "// && !_stream.HasObservers // Not supported in R3"
}
print("Patching Extenject for R3 support...", end="")
for file_path in files_to_patch:
    with fileinput.input(file_path, inplace=True) as file:
        for line in file:
            new_line = line
            for current, replacement in patch_replacements.items():
                new_line = new_line.replace(current, replacement)
            print(new_line, end="")
# also add a setter for MissingHandlerDefaultResponse property
with fileinput.input("./Assets/Plugins/Zenject/Source/Main/ZenjectSettings.cs", inplace=True) as file:
    related_getter_part = "return _missingHandlerDefaultResponse;"
    for line in file:
        print(line, end="")
        if related_getter_part in line:
            # add setter
            print("                set => _missingHandlerDefaultResponse = value;")
print("OK")

##############
# Setup finished
##############
print('\nSetup was finished successfully. Please don\'t forget to import TMP Essentials using the "Window -> '
      'TextMeshPro -> Import TMP Essential Resources" menu option')
# wait for user's confirmation before the exit
input("\nPress Enter to exit...")
