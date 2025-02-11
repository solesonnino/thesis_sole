import numpy as np
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation

# Funzione per leggere i dati dal file di testo
def read_data(filename):
    particles = []
    current_particle_data = []

    with open(filename, 'r') as file:
        lines = file.readlines()
        print("Contenuto del file:")
        print(''.join(lines))  # Stampa l'intero contenuto del file per analizzare il formato
        
        i = 0
        while i < len(lines):
            line = lines[i].strip()
            
            # Ignora le righe che non contengono dati numerici (e.g., "iteration:", "particles:", "fitness:")
            if line.startswith("iteration:") or "fitness" in line:
                i += 1
                continue  # Salta alla prossima riga

            if line.startswith("particles:"):
                i += 1
                # Iniziamo a raccogliere i dati delle particelle
                current_particle_data = []
                # Concatena le righe successive che contengono i dati delle particelle
                while i < len(lines) and lines[i].strip() and not lines[i].startswith("fitness"):
                    particle_data_line = lines[i].strip()
                    current_particle_data.append(particle_data_line)
                    i += 1
                    
                # Ora uniamo tutte le righe e creiamo un array di particelle
                particle_data = ' '.join(current_particle_data).strip()  # Unisce tutte le righe
                particle_data = particle_data.replace('][', ' ] [').replace('[', '').replace(']', '')  # Rimuove parentesi
                particle_data = particle_data.split()  # Divide in numeri

                try:
                    # Convertiamo in array numpy
                    particle_array = np.array(particle_data, dtype=int)
                    # Verifica che ci siano 20 particelle per iterazione
                    if len(particle_array) == 20:  # Verifica che ci siano 20 particelle per iterazione
                        particles.append(particle_array.reshape(20, 1))  # Rendi ogni particella una colonna
                    else:
                        print(f"Errore: la iterazione contiene {len(particle_array)} particelle, ma dovrebbe averne 20.")
                except ValueError:
                    print(f"Errore nei dati per l'iterazione {len(particles) + 1}: {particle_data}")
                    particles.append(np.array([]))  # Aggiungi un array vuoto in caso di errore nei dati
            i += 1

    # Verifica della struttura dei dati
    print(f"Numero di iterazioni: {len(particles)}")
    for idx, p in enumerate(particles):
        print(f"Iterazione {idx + 1} - Numero di particelle: {len(p)}")
    
    return particles

# Carica i dati
particles = read_data('prove_evol_1.txt')  # Sostituisci con il tuo file

# Verifica i dati caricati
for i, part in enumerate(particles):
    print(f"Iterazione {i + 1}:")
    print(part)

# Inizializza la figura e l'asse
fig, ax = plt.subplots()
sc = ax.scatter([], [], s=50)
ax.set_xlim(-700, 700)
ax.set_ylim(-10, 10)  # Mantieni y fisso a 0 per tutte le particelle
ax.set_xlabel('Posizione (x)')
ax.set_ylabel('Posizione (y)')
ax.set_title('Evoluzione delle particelle lungo l\'asse X')

# Funzione di aggiornamento per l'animazione
def update(frame):
    # Verifica che i dati delle particelle siano presenti per il frame corrente
    if len(particles[frame]) == 0:
        print(f"Attenzione: nessuna particella per il frame {frame + 1}")
        return sc,
    
    # Le particelle per l'iterazione corrente (frame)
    x = particles[frame][:, 0]  # Posizione x di tutte le particelle per il frame corrente
    y = np.zeros_like(x)  # Tutte le y sono 0
    sc.set_offsets(np.column_stack((x, y)))  # Aggiorna le posizioni delle particelle
    return sc,

# Crea l'animazione
ani = FuncAnimation(fig, update, frames=len(particles), interval=500, blit=False)

# Mostra l'animazione
plt.show()
