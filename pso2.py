''' implementazione tempo-continua del pso in cui si cerca la posizione ottimale in termini 
di manipolabilità dell'origine del sistema di riferimento della base del robot. 
il pso genera particelle la cui posizione è la posizione della base del robot e
a ogni iterazione aggiorna sulla base dell'inerzia della particella, la sua posizione.
L'inerzia di ciascuna particella è data  da un termine cognitivo e da un termine sociale '''

# creazione socket e apertura canale di comunicazione tra pso e simulatore

# Import libraries (.dll files)
import socket
import numpy as np
 
# Define the host and port to receive data (both decided by me)
host = "127.0.0.1"
port = 12345
 
# Initialize the trigger to stop the socket communication and the number of simulations to be performed
trigger_end = 0

#definition of the pso variables
# Parametri del PSO
num_particles = 5        # Number of particles
max_iterations = 5         # Number of iterations
inertia_weight = 0.5         # inertia weight
cognitive_component = 1.5    # cognitive component
social_component = 2.0       # social component
 
# Define a varibale in order to increase the number of elements to be packed and sent to C#
additional_var = 1

#initialization of the pso particles 
particle_positions = np.random.uniform(-50, 50, num_particles)  # initial positions
particle_velocities = np.random.uniform(-1, 1, num_particles)   # initial velocities
 
# Start the real communication
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client : # creation of the socket
 
    client.connect((host, port)) # connect to the server
    print("The connection has happened successfully!")

    '''here starts the actual pso implementation. At each iteration the pso will send to the simulator 
    a vector of integers, each representing the position of the i-th particle. 
    Therefore at each iteration the positions of the particles will be sent to the simulator, which runs the simulations,
    computes for each the fitness function (manipulability), and returns a vector of integers, where the i-th element represents 
    the value of manipulability (fitness) of the i-th particle'''

    
    
    
    while trigger_end < max_iterations -1 :
        print("\n")
        print(f" -*-*-*-*-*-*-*- The current simulation on TPS is the number: {trigger_end + 1} -*-*-*-*-*-*-*- ")
        print("\n")

        # recieve from the simulator something (just to start the connection)
        kpi = client.recv(1024).decode()
        kpi = [int(num) for num in kpi.split(',')] # list variable
        # Transform the data into a numpy array
        kpi_vec = np.array(kpi)
        print(f"The time and RULA score for ergonomic assessment received from C# are: {kpi_vec}")
        
        # create the array of integers by concatening particles' positions
        layout = np.array([[int(particle_positions[0]), int(particle_positions [1]),int(particle_positions [2]),int(particle_positions [3]),int(particle_positions [4])]])
        shape_layout = np.array(layout.shape, dtype = np.int32)  
        print(f"The shape of the command is : {shape_layout}")
        # Actual send of the data (in the future: try to remove the double send and try to send just one time)
        client.sendall(shape_layout.tobytes())
        client.sendall(layout.tobytes())
        print(f"particle positions: {layout}")

        #send to the simulator the positions
        

       #recieve the array of fitness
        fitness = client.recv(1024).decode()
        fitness = [int(num) for num in fitness.split(',')] # list variable
        # Transform the data into a numpy array
        fitness_Vec = np.array(fitness)
        print(f"Fitness: {fitness_Vec}") 
        par =1
        while par<=num_particles:
            temp = particle_positions [par-1]
            particle_positions[par-1]=temp+1
            par = par+1

        
        '''if trigger_end <1 : #only  at the firts iteration
            # best personal position of each particle
            personal_best_positions = particle_positions.copy()
            personal_best_scores =fitness_Vec.copy()

            # best (initial) global best position
            global_best_position = personal_best_positions[np.argmin(personal_best_scores)]
            global_best_score = np.min(personal_best_scores)

        else :
            for i in range (num_particles):
                current_fitting_value = fitness_Vec [i]

                 # update the personal best if it is necessary
                if current_fitting_value < personal_best_scores[i]:
                    personal_best_positions[i] = particle_positions[i]
                    personal_best_scores[i] = current_fitting_value
                
                # update the global best if necessary
                if current_fitting_value < global_best_score:
                    global_best_position = particle_positions[i]
                    global_best_score = current_fitting_value

        for i in range (num_particles):
            # update the velocity according to the formula
            inertia = inertia_weight * particle_velocities[i]
            cognitive = cognitive_component * np.random.random() * (personal_best_positions[i] - particle_positions[i])
            social = social_component * np.random.random() * (global_best_position - particle_positions[i])
            particle_velocities[i] = inertia + cognitive + social
                
            # update the position of the particle
            particle_positions[i] += particle_velocities[i]
            particle_positions[i] = int(particle_positions[i])  # conversione a intero'''

    
        #recieve the trigger_end
        #trigger_end = client.recv(1024).decode() 
        #trigger_end = int(''.join(map(str, trigger_end))) # transform the string into an integer
        trigger_end= fitness_Vec[num_particles]
        print(f"trigger_end value= {fitness_Vec[num_particles]}" )
        
        aspetta = np.array([[int(particle_positions[0])]])
        shape_aspetta = np.array(aspetta.shape, dtype = np.int32)  
        print(f"Taspetta shape: : {shape_layout}")
        # Actual send of the data (in the future: try to remove the double send and try to send just one time)
        client.sendall(shape_layout.tobytes())
        client.sendall(layout.tobytes())
        print(f"aspetta: {layout}")

    # close the socket 
    client.close() 

       



        



