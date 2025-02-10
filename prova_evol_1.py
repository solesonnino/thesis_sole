import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation

# 📌 1️⃣ Funzione per leggere il file e estrarre le posizioni
def leggi_dati(file_path):
    iterazioni = []  # Lista per salvare tutte le iterazioni
    with open(file_path, "r") as file:
        posizioni = []  
        for riga in file:
            riga = riga.strip()

            if riga.startswith("iteration:"):
                if posizioni:
                    iterazioni.append(np.array(posizioni))  # Salva la precedente iterazione
                    posizioni = []  # Reset per la nuova iterazione

            elif riga.startswith("particles:"):
                riga = riga.replace("particles: [[", "").replace("]]", "")  # Pulisce la stringa
                valori = list(map(int, riga.split()))  # Converte i valori in interi
                posizioni.append(valori)  # Aggiunge le posizioni

        if posizioni:  # Aggiunge l'ultima iterazione
            iterazioni.append(np.array(posizioni))

    return np.array(iterazioni)

# 📌 2️⃣ Caricare i dati dal file
file_path = "prove_evol_1.txt"  # Assicurati che il file sia nella stessa cartella
posizioni_per_iteration = leggi_dati(file_path)

# 📌 3️⃣ Impostare il grafico
fig, ax = plt.subplots()
sc = ax.scatter([], [], s=100, c='blue', label="Particelle")  # Scatter vuoto

ax.set_xlim(-700, 700)  # Imposta i limiti dell'asse X
ax.set_ylim(-700, 700)  # Imposta i limiti dell'asse Y
ax.set_xlabel("Posizione X")
ax.set_ylabel("Posizione Y")
ax.set_title("Evoluzione delle particelle (PSO)")
ax.legend()

# 📌 4️⃣ Funzione per aggiornare ogni frame
def update(frame):
    x = posizioni_per_iteration[frame][0]  # Prende solo la prima riga
    y = np.random.uniform(-500, 500, len(x))  # Se il file è 1D, generiamo una Y casuale
    sc.set_offsets(np.c_[x, y])  # Aggiorna le posizioni
    ax.set_title(f"Evoluzione delle particelle - Iterazione {frame}")
    return sc,

# 📌 5️⃣ Creare animazione
ani = animation.FuncAnimation(fig, update, frames=len(posizioni_per_iteration), interval=1000, blit=False)

plt.show()
