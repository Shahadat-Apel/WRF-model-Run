import cv2
import numpy as np
import glob
import time
import os
from pathlib import Path
 
img_array = []
sht = glob.glob('*.jpg')

fileName2 =[]
fileName3 =[]
for s in sht:
    fileName2.append(os.path.basename(s))
    #print(s)

def last_4chars(x):
    return( int(x.split('-')[0]))
path = Path(sht[0]).parent.absolute()
fileName2 = sorted(fileName2, key = last_4chars)
for s in fileName2:
    #print(os.path.join(path,s))
    fileName3.append(os.path.join(path,s))

    
for filename in fileName3:
    img = cv2.imread(filename)
    height, width, layers = img.shape
    size = (width,height)
    img_array.append(img)
 
out = cv2.VideoWriter('Amphan.avi',cv2.VideoWriter_fourcc(*'DIVX'), 1, size)
 
for i in range(len(img_array)):
    out.write(img_array[i])
    #time.sleep(10)
out.release()
print ("OK")
