from attr import s
import numpy as np
import socket


# Parametri del PSO
num_particles = 8        # Numero di particelle
max_iterations = 10         # Numero massimo di iterazioni
inertia_weight = 0.5         # Peso d'inerzia
cognitive_component = 1.5    # Componente cognitiva
social_component = 2.0       # Componente sociale

# Punto fisso noto (posizione target)
target_position = 10.0

# Inizializza le particelle con posizioni e velocità casuali lungo l'asse x
particle_positions = np.random.uniform(-50, 50, num_particles)  # Posizioni iniziali
particle_velocities = np.random.uniform(-1, 1, num_particles)   # Velocità iniziali

# Definizione della funzione di fitting (tramite comunicazione con simulatore)
def fitting_function(x):
     # Define the host and port to receive data (both decided by me)
    host = "127.0.0.1"
    port = 12345
    
    # Define a varibale in order to increase the number of elements to be packed and sent to C#
    additional_var = 1
    
    # Start the real communication
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client : # creation of the socket
    
        client.connect((host, port)) # connect to the server
        print("The connection has happened successfully!")
       
        # Invia la posizione x al client
        client.sendall()
        
        # Riceve la misura di fitting dal client
        data = client.recv(1024).decode()
        return float(data)
        
    client.close() # close the socket once the condition for the while loop is not satisfied anymore


# Migliore posizione personale di ciascuna particella
personal_best_positions = particle_positions.copy()
personal_best_scores = np.array([fitting_function(pos) for pos in personal_best_positions])

# Migliore posizione globale (inizialmente la posizione con il miglior fitting)
global_best_position = personal_best_positions[np.argmin(personal_best_scores)]
global_best_score = np.min(personal_best_scores)

# Esegui il ciclo del PSO
for iteration in range(max_iterations):
    for i in range(num_particles):
        # Calcola il valore di fitting della particella i
        current_fitting_value = fitting_function(particle_positions[i])
        
        # Aggiorna il migliore personale se il nuovo valore di fitting è migliore
        if current_fitting_value < personal_best_scores[i]:
            personal_best_positions[i] = particle_positions[i]
            personal_best_scores[i] = current_fitting_value
        
        # Aggiorna il migliore globale se necessario
        if current_fitting_value < global_best_score:
            global_best_position = particle_positions[i]
            global_best_score = current_fitting_value
        
        # Aggiornamento della velocità secondo la formula del PSO
        inertia = inertia_weight * particle_velocities[i]
        cognitive = cognitive_component * np.random.random() * (personal_best_positions[i] - particle_positions[i])
        social = social_component * np.random.random() * (global_best_position - particle_positions[i])
        particle_velocities[i] = inertia + cognitive + social
        
        # Aggiorna e arrotonda la posizione della particella alla seconda cifra decimale
        particle_positions[i] += particle_velocities[i]
        particle_positions[i] = round(particle_positions[i], 2)  # Arrotondamento a due decimali
    
    # Stampa lo stato attuale
    print(f"Iterazione {iteration + 1}/{max_iterations}, Migliore fitting globale: {global_best_score}")

# Risultato finale
print("Posizione ottima trovata:", round(global_best_position, 2))
print("Migliore valore di fitting:", round(global_best_score, 2))
