import socket
import numpy as np
from place_visualize_obj import Scene, Packer, Bin, Item

packer = Packer()
packer.add_bin(Bin('small-envelope', 78, 78, 78, 10))
packer.add_item(Item('50g [powder 1]', 26,26,26, 1))
packer.add_item(Item('50g [powder 2]', 26,26,26, 1))
packer.add_item(Item('50g [powder 3]', 26,26,26, 1))
packer.pack()
scene = Scene()

#max velocity and acceleration of the base in cm
v_max=20
a=10

items=[] #array in which i'll store all the items
for b in packer.bins:
    print(":::::::::::", b.string())
    scene.add_object_to_scene(b, False)
    print("FITTED ITEMS:")
    place_points=[]
    
    for item in b.items:
        print("====> ", item.string())
        scene.add_object_to_scene(item, False)
        place_points.append(item.get_center())
        print(item.get_center())
        items.append(item) #store into an array all the items to be picked and placed
    
    print("UNFITTED ITEMS:")
    for item in b.unfitted_items:
        print("====> ", item.string())

    print("***************************************************")
    print("***************************************************")
#scene.show_scene()
print(f"the objects will be placed in the following positions: {place_points} \n")

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

    for b in packer.bins: 
       
        for item in b.items:
             items.append(item) #store into an array all the items to be picked and placed in the considered bin
         
        #take the first item (place side)
        current_item = items[0]
        # run the pso for all the items not packed yet, evaluate them in terms of manipulability
        # once you 've found the best position of the base, for all the several objects, 
        # choose the one that takes the minimum time to be reached from the current position of the base and call it current
        # change the current position of the base and position it in the above said position called current
        # delete current from the list of items
        # repeat until items is empty

        counter=0 #counter is needed to know which item i'm packing (if it is the first, the second...) place side
        current_pos=0 #initialize the current position of the base of the robot in y=0
        pick_objects = [0,1,2] #items i have to pack, pick side

        while items:
            #run the pso for all the items inside the list items --> need to pack all the items in the bin

            #send to c# the place position associated to the item to be packed (assume all items identical)
            place_x = place_points [counter][0]
            place_y = place_points [counter][1]
            place_z = place_points [counter][2]
            # send the place point
            place_point_send= np.array ([[place_x, place_y, place_z]], dtype=np.int32)
            send_array(s,place_point_send)

            # wait for helper2, for synchronizaion purposes
            helper2=s.recv(1024).decode()
            c=0 #it tells me which object (pick side) i'm considering    
            min_time=10000 #arbitrarly large number
            

            while pick_objects: #for all the items that i have to pack (pick side), run the pso --> choose which item pick in order to place in the prescribed position
                trigger_end = 0 
                #initialization of the vector
                optimal_positions= np.zeros(num_objects)
                #initialization of the pso particles 
                particle_positions = np.random.uniform(-100, 100, num_particles)  # initial positions
                particle_velocities = np.random.uniform(-1, 1, num_particles)   # initial velocities
                
                # for each object, run the pso 
                # --> finished this loop i know the best position of the base of the robot associated to the pick of the considered item and its placement to the position i'm considering in the bin
                while trigger_end<Nsim:

                    #send the particle positions
                    #layout = np.array([[int(particle_positions[0]), int(particle_positions[1]), int(particle_positions[2]),int(particle_positions[3]),int(particle_positions[4])]], dtype= np.int32)
                    layout= np.array([[int(particle_positions[0]), int(particle_positions[1]), int(particle_positions[2])]], dtype=np.int32)
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
                    next_item=c

                c=c+1 #it tells me which object (pick side) i'm considering    

            #once i've found the association between the item that i have to pick and the place position of it,
            # i move to the next place position and look for the next item to pack
            items.remove(items[0])
            pick_objects.remove(pick_objects[next_item])
            current_pos=next_position 
            
            #i've moved the base and performed the pick and place operation, so i remove:
            # - the place point because it is taken
            # - the item pick side because it has been placed
            # Finally i update the position of the base of the robot,
            # now it is at the best position found bu the pso for that item and that place point 
            



    #print the final positions 
    print (f"positions: {optimal_positions}")

    # Close the connection
    s.close()
    optimal_positions.sort()
    print(f"optimal positions reordered: {optimal_positions}")



if __name__ == "__main__":

    # Run the code
    main()
            
