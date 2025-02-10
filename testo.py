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


# Esempio di valori di x e y che hai già
x = [0, 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8, 8.5, 9]
y = [0, 0.48, 0.84, 0.99, 0.91, 0.59, 0.14, -0.43, -0.76, -0.76, -0.43, 0.14, 0.59, 0.91, 0.99, 0.84, 0.48, 0, -0.48]

# Scrittura dei dati in un file di testo
with open('grafico.txt', 'w') as f:
    for i in range(len(x)):
        # Scrittura della coppia (x, y) e collegamento al punto successivo
        f.write(f'{x[i]:.2f},{y[i]:.2f}')
        if i < len(x) - 1:
            f.write(' -> ')  # Collegamento tra i punti
        f.write('\n')

print("File 'grafico.txt' scritto con successo!")
