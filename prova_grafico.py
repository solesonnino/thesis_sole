import matplotlib.pyplot as plt

# 1️⃣ Leggere il file di testo e pulire i dati
dati = []
with open("prova_grafico.txt", "r") as file:
    for riga in file:
        riga = riga.strip().replace(" ->", "")  # Rimuove '->'
        valori = riga.split(",")  # Divide i numeri
        if len(valori) == 2:  # Verifica che ci siano due valori
            dati.append((float(valori[0]), float(valori[1])))

# 2️⃣ Separare i dati in due liste (X e Y)
x_values, y_values = zip(*dati)  # Divide tuple in due liste separate

# 3️⃣ Creare il grafico
plt.plot(x_values, y_values, marker='o', linestyle='-', color='b', label="Dati")

# 4️⃣ Personalizzare il grafico
plt.xlabel("Tempo")
plt.ylabel("Valore")
plt.title("Grafico dei dati estratti")
plt.legend()
plt.grid(True)

# 5️⃣ Mostrare il grafico
plt.show()
