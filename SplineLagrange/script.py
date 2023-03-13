import matplotlib.pyplot as plt
import numpy as np
import os
import sys
import struct
import math

stdin = sys.stdin.buffer

def draw_plot():
    stdin.seek(0, os.SEEK_END)
    n = stdin.tell() // 16
    points = [[],[]]
    for i in range(2):
        for j in range(n):
            points[i].append(struct.unpack('d', stdin.read(8)))
    return points

points = draw_plot()

x = points[0]
y = points[1]

x1 = np.arange(0.0, 0.51, 0.01)
y1 = []
for j in x1:
    y1.append(math.exp(math.sin(j * math.pi)))

#fig, ax = plt.subplots()
#ax.set(title='Axes')

plt.figure(figsize=(12, 7))
plt.plot(x1, y1, 'b', label='function')
plt.plot(x, y, 'r', label='spline')
plt.legend();

plt.show()