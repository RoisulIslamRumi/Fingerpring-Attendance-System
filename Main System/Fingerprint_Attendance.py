import threading
import urllib2
import serial
from time import sleep

global url, fingerId

ser = serial.Serial('/dev/ttyACM0',9600)
sleep(2)

if ser.isOpen():
    print ("Connected.")
    
def sendDataToServer():
    if urllib2.urlopen(url).read():
        print ("Student is present")    

while True:
    read_serial=ser.readline()
    data=read_serial.decode()
    print (data)
    datam=data.split(',')
    if(len(datam) >= 2):
        tag=datam[0]
        fingerId=datam[1]
        if int(fingerId) > 0:
            url = "http://api.bracumongoltori.com/fingerprint.php?action=attendance&finger="+str(fingerId)
            sendDataToServer()
