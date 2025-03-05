
import socket
import numpy as np
from place_visualize_obj import Scene, Packer, Bin, Item
import os
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
import math
import random


file_path = "fixed_base_in_0.txt"

#svuoto il file se esiste
with open(file_path, 'w') as f:
    pass



#max velocity and acceleration of the base in cm
v_max=700 #mm/s
a=900 #mm/s^2

#parameters of the pso
  # Number of simulations

Num=6
Nsim = 50
trigger_end2 = 0
num_particles = 20    # Number of particles
w_0 = 0.9         # inertia weight
w_N=0.4
cognitive_component = 2    # cognitive component
social_component = 2.0 

num_types = 2
num_objects_0 = 0 #objects in the scene
num_objects_1 = 3
num_bin_0=0
num_bin_1=1

num_bins_array= [num_bin_0, num_bin_1]
num_objects_array=[num_objects_0, num_objects_1]

mean_man1= 10523.872909698997
mean_man2= 10979.224080267559
mean_man_vec=[mean_man1, mean_man2]

var_man1= math.sqrt(4262714.748894526)
var_man2= math.sqrt(1276617.8388812821)
var_man_vec=[var_man1, var_man2]

mean_t= 0.8504154353582923 
var_t= math.sqrt(0.126730866728932)

best_tradeoff = -999999999

#per diminuire linearmente il peso di inerzia da 0.9 a 0.4 divido l'intervallo per il numero di simulazioni 
#iterazione dopo iterazione vario il peso di inerzia 
delta_w=(w_N-w_0)/Nsim

#upper e lower bound della linea
upper_bound=290
lower_bound=-300

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

    place_points=[]

    # Connect to the server
    s.connect((host, port))
    print("the connection has happened succesfully \n")

    # small--> type 1
    # medium --> type 2
    packers= []
    packer1 = Packer()
    packer2 = Packer()
    packers.append(packer1)
    packers.append(packer2)
    Bin_00= Bin ('Type1_box1', 300, 200, 130, 20)
    Bin_00.set_offset(-900,-530,-107)
    packer1.add_bin(Bin_00)
    Bin_10= Bin ('Type2_box1', 300, 200, 130, 20)
    Bin_10.set_offset(-400, -530, -107)
    packer2.add_bin(Bin_10)

    packer1.add_item(Item('Cube_00', 75,150,80, 1))
    packer1.add_item(Item('Cube_01', 75,150,80, 1))
    packer1.add_item(Item('Cube_02', 75,150,80, 1))
    packer2.add_item(Item('Cube_10', 100,70,80, 1))
    packer2.add_item(Item('Cube_11', 100,70,80, 1))
    packer2.add_item(Item('Cube_12', 100,70,80, 1))

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
                x,y,z = item.get_center()
                p,r,q= b.get_offset()
                place= [x+p, y+r, q+z]
                place_points.append(place)
            print("UNFITTED ITEMS:")
            for item in b.unfitted_items:
                print("====> ", item.string())

            print("***************************************************")
            print("***************************************************")
            #scene.show_scene()


    
    current_pos= 0 #initialize the current position of the base of the robot in y=0
    layout= [-5,-5,-5,-5,-5,-5] #array in which i'll store all the optimal positions of the base for each object
    
    #send the place points
    place_point_send=np.array(place_points, dtype=np.int32)
    send_array(s, place_point_send)

    #recieve something
    helper4=s.recv(1024).decode()

    #send pick objects sequence
    pick_objects=[2,1,0,2,1,0]
    pick_obj_send= np.array([[pick_objects[0]],[pick_objects[1]],[pick_objects[2]],[pick_objects[3]],[pick_objects[4]],[pick_objects[5]]], dtype=np.int32)
    send_array(s, pick_obj_send)

    #recieve something
    helper5=s.recv(1024).decode()


    #send the sequence of base positions
    layout_send=np.array([[layout[0]],[layout[1]],[layout[2]],[layout[3]],[layout[4]],[layout[5]]], dtype=np.int32)     
    send_array(s,layout_send)

    #recieve fitness

    fitness = s.recv(1024).decode()
    fitness = [int(num) for num in fitness.split(',')] # list variable
    # Transform the data into a numpy array
    fitness_Vec= np.array(fitness)

    #write inside the file
    with open (file_path, 'a') as f:
        f.write(f"Base positions: {layout} \n Fitness values:{fitness_Vec}")

    #compute time to reach the first point
    
    global_best_position=layout[0]
    d = abs(current_pos-global_best_position)
    d_acc= pow(v_max,2)/a
    d_cost = d-d_acc
    if (d_cost <=0) : #triangular velocity profile
        t= 2*math.sqrt(d/a)
    else:
        t=2*(v_max/a)+d_cost/v_max
    
    #write 
    with open (file_path, 'a') as f:
        f.write(f"time to move: {t} \n")



    #compute the time of base motion and write in the file
    for i in range(Num-1):
        current_pos= layout[i]
        global_best_position=layout[i+1]
        d = abs(current_pos-global_best_position)
        d_acc= pow(v_max,2)/a
        d_cost = d-d_acc
        if (d_cost <=0) : #triangular velocity profile
            t= 2*math.sqrt(d/a)
        else:
            t=2*(v_max/a)+d_cost/v_max
    
        #write 
        with open (file_path, 'a') as f:
            f.write(f"time to move: {t} \n")

    s.close()

if __name__ == "__main__":

    # Run the code
    main()