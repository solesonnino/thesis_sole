import matplotlib.pyplot as plt
import csv

# 1️⃣ Leggere il file di testo e pulire i dati

            

data = []
with open("prova_grafico.txt", "r") as f:
    for line in f:
        line = line.strip()  # Rimuove spazi bianchi e newline
        if "->" in line:  
            line = line.split("->")[0]  # Se c'è " ->", prendiamo solo la parte prima
        values = line.split(",")  # Divide la stringa usando la virgola
        if len(values) == 2:  # Verifica che ci siano effettivamente due numeri
            x, y = map(float, values)
            data.append((x, y))

print(data)

# 2️⃣ Separare i dati in due liste (X e Y)
x_values, y_values = zip(*data)  # Divide tuple in due liste separate

# 3️⃣ Creare il grafico
plt.plot(x_values, y_values, marker='o', linestyle='-', color='r', label="Data")

# 4️⃣ Personalizzare il grafico
plt.xlabel("Iteration")
plt.ylabel("Fitness")
plt.title("Particle 3 fitness evolution with 150 iterations")
plt.legend()
plt.grid(True)

# 5️⃣ Mostrare il grafico
plt.show()
