import socket
import numpy as np
from place_visualize_obj import Scene, Packer, Bin, Item
import os
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
import math


# Specifica il percorso del file
file_path = "file_di_testo.txt"
file_path2= "evoluzione_particelle_pso.txt"

#svuoto il file se esiste
with open(file_path, 'w') as f:
    pass

with open(file_path2, 'w') as f:
    pass


#max velocity and acceleration of the base in cm
v_max=70 #m/s
a=90 #mm/s^2

#parameters of the pso
  # Number of simulations
Nsim = 1
trigger_end2 = 0
num_particles = 20      # Number of particles
w_0 = 0.9         # inertia weight
w_N=0.4
cognitive_component = 2    # cognitive component
social_component = 2.0 

num_types = 1
num_objects_0 = 1 #objects in the scene
num_objects_1 = 0
num_bin_0=1
num_bin_1=0

num_bins_array= [num_bin_0, num_bin_1]
num_objects_array=[num_objects_0, num_objects_1]



#per diminuire linearmente il peso di inerzia da 0.9 a 0.4 divido l'intervallo per il numero di simulazioni 
#iterazione dopo iterazione vario il peso di inerzia 
delta_w=(w_N-w_0)/Nsim

#upper e lower bound della linea
upper_bound= 290
lower_bound= -300

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

    

    # small--> type 1
    # medium --> type 2
    packers= []
    packer1 = Packer()
    packers.append(packer1)
    Bin_00= Bin ('Type1_box1', 300, 200, 130, 20)
    Bin_00.set_offset(-900,-530,-107)
    packer1.add_bin(Bin_00)


    packer1.add_item(Item('Cube_00', 75,150,80, 1))


    items=[] #array in which i'll store all the items

    for packer in packers:
        packer.pack()
        for b in packer.bins:
            scene= Scene()
            print(":::::::::::", b.string())
            scene.add_object_to_scene(b, False)
            print("FITTED ITEMS:")

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


    type_obj=0
    current_pos=0 #initialize the current position of the base of the robot in y=0
    base_position_sequence= [] #array in which i'll store all the optimal positions of the base for each object

    while type_obj<num_types:
        packer=packers[type_obj]
        num_bins= num_bins_array[type_obj]
        bin=0
        num_obj_pick=num_objects_array[type_obj]
        pick_objects = [] #array in which i'll store the objects, pick side, that i've already picked and placed

        #send the zoffset of the top face wrt to the center of the object of the considered type
        generic_bin= packer.bins[0]
        generic_item=generic_bin.items[0]
        z_top_face = (generic_item.depth)/2
        print(f"z_topface  {z_top_face}" )
        z_top_face_send= np.array ([[z_top_face]], dtype=np.int32)
        send_array(s,z_top_face_send)

        print(f"type= {type_obj} \n" )
            
        #recieve something
        helper0=s.recv(1024).decode()

        while bin<num_bins: 

            place_points=[]
            rotations=[]
            k=0
            b = packer.bins[bin]
            x_offset_box, y_offset_box, z_offset_box = b.get_offset()


            for item in b.items:
                items.append(item) #store into an array all the items to be picked and placed in the considered bin
                k=k+1
                place_points.append(item.get_center()) 
                rotations.append (item.rotation_type)

            print(f"the objects will be placed in the following positions: {place_points} \n")
            num_objects = k #number of objects inside the considered  bin

            num_objects_send= np.array ([[num_objects]], dtype=np.int32)
            send_array(s,num_objects_send)
            
            #recieve something
            helper5=s.recv(1024).decode()

            # run the pso for all the items not packed yet, evaluate them in terms of manipulability
            # once you 've found the best position of the base, for all the several objects, 
            # choose the one that takes the minimum time to be reached from the current position of the base and call it current
            # change the current position of the base and position it in the above said position called current
            # delete current from the list of items
            # repeat until items is empty
            
            
            i=0
            

            while i<num_objects:
                #run the pso for all the items inside the list items --> need to pack all the items in the bin
                print(f"currently finding the item number: {i} \n")
                current_item=items[i]

                #send to c# the place position associated to the item to be packed (assume all items identical)
                place_x = place_points [i][0] + x_offset_box
                place_y = place_points [i][1] + y_offset_box
                place_z = place_points [i][2] + z_offset_box
                # send the place point
                place_point_send= np.array ([[place_x, place_y, place_z]], dtype=np.int32)
                send_array(s,place_point_send)

                #recieve something 
                helper4=s.recv(1024).decode()

                #send the rotation associated with the object
                rotation= rotations[i]
                rotation_send= np.array ([[rotation]], dtype=np.int32)
                send_array(s,rotation_send)            

                # wait for helper2, for synchronizaion purposes
                helper2=s.recv(1024).decode()

                c=0 #it tells me which object (pick side) i'm considering    
                min_time=10000 #arbitrarly large number

                #define particle1_x
                particle1_x = np.zeros(Nsim)
                #define particle1_y
                particle1_y=np.zeros(Nsim)

                # inizializzo il vettore dove metto l'evoluzione delle particelle dello sciame
                swarm_evolution = [[] for _ in range (Nsim)]
                x_swarm= [[] for _ in range (Nsim)]

                

                while c<num_obj_pick: 
                    #for all the items that i have to pack (pick side), run the pso --> choose which item pick in order to place in the prescribed position
                    if c not in pick_objects : 
                        #if the object has not ever been picked, then send skip=0, and perform all the computations    
                        skip=np.array([[0]], np.int32)
                        send_array(s,skip)
                        #recieve something
                        helper=s.recv(1024).decode()
                        trigger_end = 0 
                        #initialization of the pso particles 
                        particle_positions = np.random.uniform(lower_bound, upper_bound, num_particles)  # initial positions
                        particle_velocities = np.random.uniform(-1, 1, num_particles)   # initial velocities
                        # for each object, run the pso 
                        # --> finished this loop i know the best position of the base of the robot associated to the pick of the considered item and its placement to the position i'm considering in the bin
                        #initialize the inertia weight
                        w_0=0.9
                        while trigger_end<Nsim:
                            #update the inertia weigth at each iteration
                            inertia_weight=w_0+delta_w*trigger_end
                            print(f"inertia weight: {inertia_weight}")
                            #send the particle positions
                            layout = np.array([[int(particle_positions[0]), int(particle_positions[1]), int(particle_positions[2]),int(particle_positions[3]),int(particle_positions[4]),  int(particle_positions[5]), int(particle_positions[6]), int(particle_positions[7]), int(particle_positions[8]), int(particle_positions[9]), int(particle_positions[10]), int(particle_positions[11]), int(particle_positions[12]), int(particle_positions[13]), int(particle_positions[14]), int(particle_positions[15]), int(particle_positions[16]), int(particle_positions[17]), int(particle_positions[18]), int(particle_positions[19]),]], dtype= np.int32)
                            #layout = np.array([[int(particle_positions[0])]], dtype= np.int32)
                            # Actual send of the data (in the future: try to remove the double send and try to send just one time)
                            send_array(s,layout)
                            print(f"particle positions: {layout}")

                            with open(file_path2, "a") as File:
                                File.write(f"iteration: {trigger_end} \n particles: {layout}\n")

                            #recieve the fitness
                            fitness = s.recv(1024).decode()
                            fitness = [int(num) for num in fitness.split(',')] # list variable
                            # Transform the data into a numpy array
                            fitness_Vec= np.array(fitness)
                            #print(f"the fitness values are: {fitness_Vec} \n")
                            for l in range (num_particles):
                                if fitness_Vec[i]>99999:
                                    fitness_Vec[i]=0

                            with open(file_path2, "a") as File:
                                File.write(f"fitness: {fitness_Vec} \n\n")

                            #send something just to see
                            helper3= np.array([[0]], dtype=np.int32)
                            # Actual send of the data (in the future: try to remove the double send and try to send just one time)
                            send_array(s,helper3)

                            # Receive the variable 'trigger_end' from C# code
                            trigger_end = int(s.recv(1024).decode())
                            print(f"Trigger end: {trigger_end}")

                            #save the updates of the second particle along the simulation for the first object pick side
                            if (c==0): 
                                #save the fitness evolution
                                particle1_x[trigger_end - 1]= trigger_end - 1 #sottraggo 1 perche l'ho già ricevuto
                                particle1_y[trigger_end - 1]=fitness_Vec[1]

                                #save the swarm evolution for the first object pick side
                                swarm_evolution[trigger_end -1] = particle_positions.copy()
                                x_swarm[trigger_end-1] = np.zeros(num_particles)


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

                                #controllo e riposizionamento dentro i limiti
                                if (particle_positions[i]>upper_bound):
                                    particle_positions[i]=upper_bound

                                if (particle_positions[i]<lower_bound):
                                    particle_positions[i]=lower_bound    






                        #send something just to see
                        helper3= np.array([[0]], dtype=np.int32)
                        # Actual send of the data (in the future: try to remove the double send and try to send just one time)
                        send_array(s,helper3)    

                    else :
                        skip= np.array([[1]], dtype=np.int32)
                        send_array(s,skip)


                    #move to the next 
                    # #recieve c
                    c= int(s.recv(1024).decode())

                #once i've found the association between the item that i have to pick and the place position of it,
                # i move to the next place position and look for the next item to pack
                
                #print(f"object: {next_item} \n has been positioned inside the box {bin} at the position: {current_item.get_center()}")
                #print(f"the optimal position of the base is: {current_pos}")
                

                #once chosen, add the item in the list of the objects already picked
                

                #the position of the base
                base_position_sequence.append(current_pos)

                #send something just to see
                helper3= np.array([[0]], dtype=np.int32)
                # Actual send of the data (in the future: try to remove the double send and try to send just one time)
                send_array(s,helper3)
                i= int(s.recv(1024).decode())
                print(f"\ni= {i}\n")
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

        #send something
        helper15= np.array([[0]], dtype=np.int32)
        send_array(s,helper15)

        #recieve type
        type_obj=int(s.recv(1024).decode())


    # Close the connection
    s.close()

    #print the graph of the particle fitness evolution considered
    grafico_path="grafico.txt"
    if os.path.exists(grafico_path):
    # Cancella il file
        os.remove(grafico_path)

    with open('grafico.txt', 'w') as f:
        for iter in range(len(particle1_x)):
            # Scrittura della coppia (x, y) e collegamento al punto successivo
            f.write(f'{particle1_x[iter]:.2f},{particle1_y[iter]:.2f}')
            if iter < len(particle1_x) - 1:
                f.write(' -> ')  # Collegamento tra i punti
                f.write('\n')

            #visualizzazione grafico
        plt.figure()  # Crea una nuova figura
        plt.plot(particle1_x, particle1_y, marker='o', color='r', label='Grafico 1')
        plt.title('Grafico 1')
        plt.xlabel('Asse X')
        plt.ylabel('Asse Y')
        plt.grid(True)
        plt.legend()

    # print the evolution of the swarm considered
    fig, ax = plt.subplots()
    scatter = ax.scatter(x_swarm[0], swarm_evolution[0], c='blue', s=50)
    ax.set_title("Swarm evolution")

    # Funzione per aggiornare il grafico in ogni frame
    def update(frame):
        scatter.set_offsets(np.c_[x_swarm[frame], swarm_evolution[frame]])  # Aggiorna le posizioni
        y_positions = swarm_evolution[frame]  # Prendi le posizioni lungo y
        y_min, y_max = lower_bound, upper_bound  # Calcola i limiti dinamici di y
        x_positions = x_swarm[frame]  # Prendi le posizioni lungo y
        x_min, x_max = x_positions.min(), x_positions.max()  # Calcola i limiti dinamici di y

        ax.set_ylim(y_min - 5, y_max + 5)  # Aggiungi margine dinamico ai limiti di y
        ax.set_xlim(x_min-5, x_max +5)

        ax.set_title(f"Iterazione {frame + 1}/{Nsim}")
        plt.draw()

        return scatter,

    # Creazione dell'animazione
    ani = FuncAnimation(fig, update, frames=Nsim, interval=200, blit=False, repeat=False)

    # Mostra tutto
    plt.show()
        

    #summarize all the choices 
    print(f"the global best is: {global_best_position} \n manipulability: {global_best_score}")


if __name__ == "__main__":

    # Run the code
    main()
            
