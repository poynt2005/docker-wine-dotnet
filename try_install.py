import threading
import subprocess
import re
import os
import time

from xvfb_checker import *
import change_win_version


def try_install():
    try_times = 0

    while try_times < 20:
        os.system('echo "Checking xvfb running for %d times"' % (try_times))

        thr = threading.Thread(target=run_entrypoint)
        thr.daemon = True
        thr.start()

        time.sleep(2)
        is_running = check_xvfb_running()

        if is_running:
            break

        try_times = try_times + 1
        time.sleep(2)

    os.system('echo "Change OS to windows 10"')
    change_win_version.change_win_version('win10')

    os.system('echo "Install webview2"')
    os.system('timeout 10m wine webview2.exe /silent /install')

    os.system('echo "Install riched20"')
    os.system('timeout 10m winetricks --unattended --force riched20')


if __name__ == '__main__':
    try_install()
