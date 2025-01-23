import socket
import numpy as np
from place_visualize_obj import Scene, Packer, Bin, Item
import os
import matplotlib.pyplot as plt


# Specifica il percorso del file
file_path = "file_di_testo.txt"

# Controlla se il file esiste
if os.path.exists(file_path):
    # Cancella il file
    os.remove(file_path)
    print(f"Il file '{file_path}' è stato cancellato.")
else:
    print(f"Il file '{file_path}' non esiste.")


#max velocity and acceleration of the base in cm
v_max=20
a=10

#parameters of the pso
  # Number of simulations
Nsim = 5
trigger_end2 = 0
num_particles = 5      # Number of particles
inertia_weight = 0.5         # inertia weight
cognitive_component = 1.5    # cognitive component
social_component = 2.0 
num_objects = 1 #objects in the scene


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

    packer = Packer()
    packer.add_bin(Bin('small-envelope', 100, 100, 100, 20))
    packer.add_item(Item('50g [powder 1]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 2]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 2]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 2]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 3]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 3]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 3]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 3]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 3]', 25,25,25, 1))
    packer.add_item(Item('50g [powder 3]', 25,25,25, 1))
    packer.pack()
    scene = Scene()
    items=[] #array in which i'll store all the items
    for b in packer.bins:
        print(":::::::::::", b.string())
        scene.add_object_to_scene(b, False)
        print("FITTED ITEMS:")
        place_points=[]
        
        for item in b.items:
            print("====> ", item.string())
            scene.add_object_to_scene(item, False)
            print(item.get_center())
            items.append(item) #store into an array all the items to be picked and placed
        
        print("UNFITTED ITEMS:")
        for item in b.unfitted_items:
            print("====> ", item.string())

        print("***************************************************")
        print("***************************************************")
        #scene.show_scene()
    bin=0
    num_bins=1
    while bin<num_bins: 
       
        b = packer.bins[bin]
        for item in b.items:
            items.append(item) #store into an array all the items to be picked and placed in the considered bin
            place_points.append(item.get_center())
        print(f"the objects will be placed in the following positions: {place_points} \n")

        # run the pso for all the items not packed yet, evaluate them in terms of manipulability
        # once you 've found the best position of the base, for all the several objects, 
        # choose the one that takes the minimum time to be reached from the current position of the base and call it current
        # change the current position of the base and position it in the above said position called current
        # delete current from the list of items
        # repeat until items is empty
        
        current_pos=0 #initialize the current position of the base of the robot in y=0
        pick_objects = [] #array in which i'll store the objects, pick side, that i've already picked and placed
        base_position_sequence= [] #array in which i'll store all the optimal positions of the base for each object
        i=0
        while i<num_objects:
            #run the pso for all the items inside the list items --> need to pack all the items in the bin
            print(f"currently finding the item number: {i} \n")
            current_item=items[i]

            #send to c# the place position associated to the item to be packed (assume all items identical)
            place_x = place_points [i][0]
            place_y = place_points [i][1]
            place_z = place_points [i][2]
            # send the place point
            place_point_send= np.array ([[place_x, place_y, place_z]], dtype=np.int32)
            send_array(s,place_point_send)

            # wait for helper2, for synchronizaion purposes
            helper2=s.recv(1024).decode()
            c=0 #it tells me which object (pick side) i'm considering    
            min_time=10000 #arbitrarly large number
            

            while c<num_objects: 
                #for all the items that i have to pack (pick side), run the pso --> choose which item pick in order to place in the prescribed position
                if c not in pick_objects : 
                    #clear particle_x
                    particle_x = np.zeros(Nsim)
                    #clear particle_y
                    particle_y=np.zeros(Nsim)
                    #if the object has not ever been picked, then send skip=0, and perform all the computations    
                    skip=np.array([[0]], np.int32)
                    send_array(s,skip)
                    #recieve something
                    helper=s.recv(1024).decode()
                    trigger_end = 0 
                    #initialization of the pso particles 
                    particle_positions = np.random.uniform(-100, 100, num_particles)  # initial positions
                    particle_velocities = np.random.uniform(-1, 1, num_particles)   # initial velocities
                    
                    # for each object, run the pso 
                    # --> finished this loop i know the best position of the base of the robot associated to the pick of the considered item and its placement to the position i'm considering in the bin
                    while trigger_end<Nsim:

                        #send the particle positions
                        layout = np.array([[int(particle_positions[0]), int(particle_positions[1]), int(particle_positions[2]),int(particle_positions[3]),int(particle_positions[4])]], dtype= np.int32)
                        #layout = np.array([[int(particle_positions[0])]], dtype= np.int32)
                        # Actual send of the data (in the future: try to remove the double send and try to send just one time)
                        send_array(s,layout)
                        print(f"particle positions: {layout}")

                        #recieve the fitness
                        fitness = s.recv(1024).decode()
                        fitness = [int(num) for num in fitness.split(',')] # list variable
                        # Transform the data into a numpy array
                        fitness_Vec= np.array(fitness)
                        print(f"the fitness values are: {fitness_Vec} \n")

                        #send something just to see
                        helper3= np.array([[0]], dtype=np.int32)
                        # Actual send of the data (in the future: try to remove the double send and try to send just one time)
                        send_array(s,helper3)

                        # Receive the variable 'trigger_end' from C# code
                        trigger_end = int(s.recv(1024).decode())
                        print(f"Trigger end: {trigger_end}")

                        #update the particles positions 

                        #set pbest and gbest
                        if trigger_end ==1 : #only  at the firts iteration
                            # best personal position of each particle
                            personal_best_positions = particle_positions.copy()
                            personal_best_scores =fitness_Vec.copy()

                            # best (initial) global best position
                            global_best_position = personal_best_positions[np.argmax(personal_best_scores)]
                            global_best_score = np.max(personal_best_scores)

                        else :
                            for i in range (num_particles):
                                current_fitting_value = fitness_Vec [i]

                                # update the personal best if it is necessary
                                if current_fitting_value > personal_best_scores[i]:
                                    personal_best_positions[i] = particle_positions[i]
                                    personal_best_scores[i] = current_fitting_value
                                
                                # update the global best if necessary
                                if current_fitting_value > global_best_score:
                                    global_best_position = particle_positions[i]
                                    global_best_score = current_fitting_value

                        #update particles            
                        for i in range (num_particles):
                            # update the velocity according to the formula
                            inertia = inertia_weight * particle_velocities[i]
                            cognitive = cognitive_component * np.random.random() * (personal_best_positions[i] - particle_positions[i])
                            social = social_component * np.random.random() * (global_best_position - particle_positions[i])
                            particle_velocities[i] = inertia + cognitive + social
                                
                            # update the position of the particle
                            particle_positions[i] += particle_velocities[i]
                            particle_positions[i] = int(particle_positions[i])  # conversione a intero
                        
                        #save the updates of the second particle along the simulation for the second object
                        if (c==0): 
                            particle_x[trigger_end - 1]= trigger_end - 1 #sottraggo 1 perche l'ho già ricevuto
                            particle_y[trigger_end - 1]=fitness_Vec[1]

                    #print the graph of the particle evolution considered
                    grafico_path="grafico.txt"
                    if os.path.exists(grafico_path):
                    # Cancella il file
                        os.remove(grafico_path)

                    with open('grafico.txt', 'w') as f:
                        for iter in range(len(particle_x)):
                            # Scrittura della coppia (x, y) e collegamento al punto successivo
                            f.write(f'{particle_x[iter]:.2f},{particle_y[iter]:.2f}')
                            if iter < len(particle_x) - 1:
                                f.write(' -> ')  # Collegamento tra i punti
                            f.write('\n')

                    #visualizzazione grafico
                    plt.figure()  # Crea una nuova figura
                    plt.plot(particle_x, particle_y, marker='o', color='r', label='Grafico 1')
                    plt.title('Grafico 1')
                    plt.xlabel('Asse X')
                    plt.ylabel('Asse Y')
                    plt.grid(True)
                    plt.legend()
                    plt.show()  # Mostra il primo grafico



                    # evaluate the time needed to move the base from the current position to the one i'm evaluating
                    d = abs(current_pos-global_best_position)
                    d_acc= pow(v_max,2)/a
                    d_cost = d-2*d_acc
                    if (d_cost <=0) : #triangular velocity profile
                        t=v_max/a
                    else:
                        t=2*(v_max/a)+d_cost/v_max    
                    
                    if (t < min_time): #if the current motion is better, update
                        min_time=t
                        next_position = global_best_position
                        print(f"\npartial computation: {next_position}\n")
                        next_item=c

                    #send something just to see
                    helper3= np.array([[0]], dtype=np.int32)
                    # Actual send of the data (in the future: try to remove the double send and try to send just one time)
                    send_array(s,helper3)    

                else :
                    skip= np.array([[1]], dtype=np.int32)
                    send_array(s,skip)


                #move to the next
                c= int(s.recv(1024).decode())

            #once i've found the association between the item that i have to pick and the place position of it,
            # i move to the next place position and look for the next item to pack
            current_pos=next_position 
            print(f"object: {next_item} \n has been positioned inside the box {bin} at the position: {current_item.get_center()}")
            print(f"the optimal position of the base is: {current_pos}")
            
            # Creare un file e scrivere del testo
            with open("file_di_testo.txt", "w") as File:
                File.write(f"object: {next_item} \n has been positioned inside the box {bin} at the position: {current_item.get_center()}")
                File.write(f"the optimal position of the base is: {current_pos}")

            #once chosen, add the item in the list of the objects already picked
            pick_objects.append(next_item)

            #the position of the base
            base_position_sequence.append(current_pos)

            #send something just to see
            helper3= np.array([[0]], dtype=np.int32)
            # Actual send of the data (in the future: try to remove the double send and try to send just one time)
            send_array(s,helper3)
            i= int(s.recv(1024).decode())
            #i've moved the base and performed the pick and place operation, so i remove:
            # - the place point because it is taken
            # - the item pick side because it has been placed
            # Finally i update the position of the base of the robot,
            # now it is at the best position found bu the pso for that item and that place point 

        #send something just to see
        helper3= np.array([[0]], dtype=np.int32)
        # Actual send of the data (in the future: try to remove the double send and try to send just one time)
        send_array(s,helper3)
        bin= int(s.recv(1024).decode())       

    # Close the connection
    s.close()

    #summarize all the choices 
    print(f"the sequence at which the objects will be taken in order to minimize the time of motion of the base is: {pick_objects} and the sequence of positions of the base is: {base_position_sequence}")


if __name__ == "__main__":

    # Run the code
    main()
            
