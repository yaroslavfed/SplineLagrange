import matplotlib.pyplot as plt
import numpy as np
import pylab
import os
import sys
import struct
import math

step = 0.01
x1 = np.arange(-4, 4 + step, step)

y1 = []
for j in x1:
    y1.append(1/(1+25*math.pow(j, 2)))
    #y1.append(math.fabs(j))
    #y1.append(math.pow(math.fabs(j), math.sin(math.pi * j)))
    #y1.append(math.exp(math.sin(j * math.pi)))
    #y1.append(math.pow(j, 3))

xp = []
yp = []

yf = []
xf = []

with open("test.txt","r") as f:
    xp = f.readline()[:-2].replace(',','.')
    yp = f.readline()[:-2].replace(',','.')
    xf = f.readline()[:-2].replace(',','.')
    yf = f.readline()[:-2].replace(',','.')

xp=list(map(float, xp.split(' ')))
yp=list(map(float, yp.split(' ')))
xf=list(map(float, xf.split(' ')))
yf=list(map(float, yf.split(' ')))

plt.plot(x1, y1, 'b', label='function')
plt.plot(xf, yf, 'r', label='spline')
plt.plot(xp, yp, 'ro')

plt.legend()

plt.show()