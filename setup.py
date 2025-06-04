import subprocess
import sys
import shutil
import os

default_cwd = os.getcwd()

def run_git(command):
    try:
        subprocess.run(['git'] + command.split())
    except:
        print('Error: git is not available')
        sys.exit(1)


def get_repo(name, origin, repo_path, commit):
    print('--- Getting ' + name + ' repo:')

    # clear repo folder if it is not empty
    repo_dir = os.listdir(repo_path)
    if len(repo_dir) > 0:
        shutil.rmtree(repo_path)
        os.mkdir(repo_path)

    run_git('clone ' + origin + ' ' + repo_path)
    os.chdir(repo_path)
    run_git('checkout ' + commit)
    os.chdir(default_cwd)

print('--- CSharp')
dotween_repo_path = './csharp_unity/ThirdParty/DOTween'
get_repo('DOTween', 'https://github.com/Demigiant/dotween.git', dotween_repo_path, '47e873e')

print('--- Copying DOTween lib to Assets...')
dotween_lib_path = './csharp_unity/Assets/DOTween'
if os.path.isdir(dotween_lib_path):
    # remove old directory
    shutil.rmtree(dotween_lib_path)
shutil.copytree(dotween_repo_path + '/_DOTween.Assembly/bin', dotween_lib_path)

print('--- Done\n')
