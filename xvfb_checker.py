import threading
import subprocess
import re
import os
import time


def check_xvfb_running():
    sb = subprocess.Popen(
        ['ps', '-aux'], stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=False)
    ps_result = sb.stdout.read().decode('utf-8').split('\n')

    for line in ps_result:
        ps_line_result_split = re.split('\\s+', line)
        if len(ps_line_result_split) < 10:
            continue
        ps_cmd = ' '.join(ps_line_result_split[10:]).strip()

        if re.search('\/bin\/xvfb', ps_cmd, re.IGNORECASE):
            return True
    return False


def close_all_adobe():
    sb = subprocess.Popen(
        ['ps', '-aux'], stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=False)
    ps_result = sb.stdout.read().decode('utf-8').split('\n')

    for line in ps_result:
        ps_line_result_split = re.split('\\s+', line)
        if len(ps_line_result_split) < 10:
            continue
        ps_cmd = ' '.join(ps_line_result_split[10:]).strip()

        if not re.search('Adobe', ps_cmd, re.IGNORECASE):
            continue

        pid = ps_line_result_split[1]

        print('Found PID: %d, killing...')
        os.system('kill -9 %s' % pid)


def run_entrypoint():
    os.system('entrypoint')
    time.sleep(2)
