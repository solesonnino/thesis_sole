import socket
import numpy as np

host = "127.0.0.1"
port = 12345
trigger_end = 0

# Parametri del PSO
num_particles = 5
max_iterations = 5
inertia_weight = 0.5
cognitive_component = 1.5
social_component = 2.0

# Inizializzazione del PSO
particle_positions = np.random.uniform(-50, 50, num_particles)
particle_velocities = np.random.uniform(-1, 1, num_particles)

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client:
    client.connect((host, port))
    print("Connessione al simulatore stabilita!")

    # Inizializza i migliori valori personali e globali
    personal_best_positions = particle_positions.copy()
    personal_best_scores = np.full(num_particles, np.inf)
    global_best_position = personal_best_positions[0]
    global_best_score = np.inf

    for iteration in range(max_iterations):
        print(f"--- Iterazione PSO: {iteration + 1} ---")

        # Ricezione dei KPI (prima ricezione)
        kpi = client.recv(1024).decode()
        kpi_vec = np.array([int(num) for num in kpi.split(',')])

        # Invio delle posizioni delle particelle al simulatore
        layout = np.array([[int(pos) for pos in particle_positions]])
        shape_layout = np.array(layout.shape, dtype=np.int32)
        client.sendall(shape_layout.tobytes())
        client.sendall(layout.tobytes())

        # Ricezione della fitness
        fitness = client.recv(1024).decode()
        fitness_Vec = np.array([int(num) for num in fitness.split(',')])

        # Aggiornamento dei migliori valori personali e globali
        for i in range(num_particles):
            if fitness_Vec[i] < personal_best_scores[i]:
                personal_best_positions[i] = particle_positions[i]
                personal_best_scores[i] = fitness_Vec[i]
            if fitness_Vec[i] < global_best_score:
                global_best_position = particle_positions[i]
                global_best_score = fitness_Vec[i]

        # Aggiornamento velocità e posizione delle particelle
        for i in range(num_particles):
            inertia = inertia_weight * particle_velocities[i]
            cognitive = cognitive_component * np.random.random() * (personal_best_positions[i] - particle_positions[i])
            social = social_component * np.random.random() * (global_best_position - particle_positions[i])
            particle_velocities[i] = inertia + cognitive + social
            particle_positions[i] += particle_velocities[i]

        # Imposta il trigger_end per la terminazione
        trigger_end += 1

    client.close()
