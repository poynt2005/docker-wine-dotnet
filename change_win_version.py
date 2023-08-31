import threading
import os
import time


def change_win_version(version: str):
    def make_cfg_thread():
        cf_thread = threading.Thread(target=lambda: os.system('winecfg'))
        cf_thread.daemon = True
        cf_thread.start()

    version_mapping = {
        'win10': '194 414',
        'win8.1': '188 426',
        'win8': '201 438',
        'win2008R2': '199 453',
        'win7': '194 464',
        'win2008': '203 478',
        'winVista': '196 492',
        'win2003': '191 504',
        'winxp': '184 519'
    }

    if not version in version_mapping:
        raise TypeError('Target version: %s not supported' % version)

    make_cfg_thread()
    time.sleep(5)
    os.system('xdotool mousemove 245 394 click 1')
    time.sleep(1)
    os.system('xdotool mousemove %s click 1' % version_mapping[version])
    time.sleep(1)
    os.system('xdotool mousemove 208 448 click 1')
    time.sleep(3)
