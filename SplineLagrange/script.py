import matplotlib.pyplot as plt
import numpy as np
import os
import sys
import struct

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
y1 = x

fig, ax = plt.subplots()
ax.set(title='Axes')
ax.plot(x, y)
ax.plot(x, y1)
plt.show()