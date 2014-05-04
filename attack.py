from netaddr import IPNetwork
import os
for ip in IPNetwork ('x.x.x.x/24'):
    os.system("python code.py "+str(ip))
#os.system("python code.py 192.168.1.1")
