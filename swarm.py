import numpy as np
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation

# Simulazione di dati dello sciame
num_points = 50
num_frames = 5

# Generazione dei dati dello sciame
x_data = np.cumsum(np.random.uniform(-0.5, 0.5, (num_frames, num_points)), axis=0)
y_data = np.cumsum(np.random.uniform(-0.5, 0.5, (num_frames, num_points)), axis=0)

# Creazione della figura
fig, ax = plt.subplots()
sc = ax.scatter(x_data[0], y_data[0], c='blue', s=50)  # Punti iniziali
ax.set_title("Animazione dello sciame di punti")

# Funzione per aggiornare i punti e i limiti ad ogni frame
def update(frame):
    sc.set_offsets(np.c_[x_data[frame], y_data[frame]])  # Aggiorna le posizioni dei punti

    # Calcolo dinamico dei limiti
    x_min, x_max = x_data[frame].min(), x_data[frame].max()
    y_min, y_max = y_data[frame].min(), y_data[frame].max()
    ax.set_xlim(x_min - 1, x_max + 1)  # Aggiungi un margine
    ax.set_ylim(y_min - 1, y_max + 1)  # Aggiungi un margine
    return sc,

# Creazione dell'animazione
ani = FuncAnimation(fig, update, frames=num_frames, interval=50, blit=True)

# Mostra l'animazione
plt.show()
