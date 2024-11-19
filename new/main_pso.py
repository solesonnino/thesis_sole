""" 
Test1: more than one array is sent to C# (in sequence)
"""

import socket
import numpy as np

def send_array(sock, array):
    # Send the shape and type of the array first
    shape = np.array(array.shape, dtype=np.int32)
    sock.sendall(shape.tobytes())
    sock.sendall(array.tobytes())

def main():

    # Create a socket object
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    # Define the host and port
    host = '127.0.0.1'
    port = 12345

    # Connect to the server
    s.connect((host, port))
    print("the connection has happened succesfully \n")

    # Number of simulations
    Nsim = 5
    trigger_end = 0
    num_particles = 5        # Number of particles
    inertia_weight = 0.5         # inertia weight
    cognitive_component = 1.5    # cognitive component
    social_component = 2.0 

    #initialization of the pso particles 
    particle_positions = np.random.uniform(-50, 50, num_particles)  # initial positions
    particle_velocities = np.random.uniform(-1, 1, num_particles)   # initial velocities


    while trigger_end < Nsim - 1:

        #send the particle positions
        layout = np.array([[int(particle_positions[0]), int(particle_positions[1]),int(particle_positions[2]),int(particle_positions[3]),int(particle_positions[4])]], dtype= np.int32)

        # Actual send of the data (in the future: try to remove the double send and try to send just one time)
        send_array(s,layout)
        print(f"particle positions: {layout}")

        #recieve the fitness
        fitness = s.recv(1024).decode()
        fitness = [int(num) for num in fitness.split(',')] # list variable
        # Transform the data into a numpy array
        fitness_Vec= np.array(fitness)
        print(f"the fitness values are: {fitness_Vec} \n")

        # Receive the variable 'trigger_end' from C# code
        trigger_end = int(s.recv(1024).decode())
        print(f"Trigger end: {trigger_end}")

    # Close the connection
    s.close()

if __name__ == "__main__":

    # Run the code
    main()