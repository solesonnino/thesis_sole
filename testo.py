import os
import matplotlib.pyplot as plt
import numpy as np

# Generazione dei dati
t = np.arange(0, 10, 0.5)  # Istanti di tempo discretizzati (esempio: da 0 a 10 con passo 0.5)
y = np.sin(t)  # I valori di y corrispondenti, in questo caso la funzione seno

# Scrittura dei dati in un file di testo
with open('grafico.txt', 'w') as f:
    for i in range(len(t)):
        # Scrittura della coppia (x, y) e collegamento al punto successivo
        f.write(f'{t[i]:.2f},{y[i]:.2f}')
        if i < len(t) - 1:
            f.write(' -> ')
        f.write('\n')

print("File 'grafico.txt' scritto con successo!")


# Visualizzazione del grafico
plt.plot(t, y, marker='o', linestyle='-', color='b')
plt.xlabel('Tempo')
plt.ylabel('Valore')
plt.title('Grafico XY')
plt.grid(True)
plt.show()
