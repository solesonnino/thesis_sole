#pick random object from the available ones
#select random place position from the available ones
#select a random position for the base of the robot
#store the mean manipulability 

import socket
import numpy as np
from place_visualize_obj import Scene, Packer, Bin, Item
import os
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
import random
import statistics

def send_array(sock, array):
    # Send the shape and type of the array first
    shape = np.array(array.shape, dtype=np.int32)
    sock.sendall(shape.tobytes())
    sock.sendall(array.tobytes())

def main():

    #start connection  
    # Create a socket object
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    print("socket creata, ora cerco il server\n")

    # Define the host and port
    host = '127.0.0.1'
    port = 12345

    # Connect to the server
    s.connect((host, port))
    print("the connection has happened succesfully \n")


    # Specifica il percorso del file
    file_path3="single_object_upper_bound.txt"

    #svuoto il file se esiste
    with open(file_path3, 'w') as f:
        pass





    Nset=10
    i=0
    lower_bound=300
    upper_bound=310

    items=[]
    place_points=[]
    dati=[]
    unusable=[]

    packer1 = Packer()
    b=Bin ('Type1_box1', 300, 200, 130, 20)
    b.set_offset(-900,-530,-107)
    packer1.add_bin(b)
    packer1.add_item(Item('Cube_00', 75,150,80, 1))
    packer1.pack()

    x_offset_box, y_offset_box, z_offset_box = b.get_offset()
    for item in b.items:
        items.append(item)
        place_points.append(item.get_center()) 
        print(f"{place_points} \n")
    

   
    # loop
    while(i<Nset):

        #manda posizione casuale della base del robot
        base=random.uniform(lower_bound, upper_bound)
        base_send= np.array ([[base]], dtype=np.int32)
        send_array(s,base_send)

        #ricevi qualcosa
        helper1=s.recv(1024).decode()

        place_x = place_points [0][0] + x_offset_box
        place_y = place_points [0][1] + y_offset_box
        place_z = place_points [0][2] + z_offset_box + 40
        print(f"place: {place_x} {place_y} {place_z}")

        #manda la pos di place
        place_point_send= np.array ([[place_x, place_y, place_z]], dtype=np.int32)
        send_array(s,place_point_send)

        #ricevi trigger_end (i)
        i= int(s.recv(1024).decode())

        #manda info sull'ogg di pick
        pick=0
        pick_send= np.array ([[pick]], dtype=np.int32)
        send_array(s,pick_send)

        #ricevi la manipolabilità
        Mean_determinant = int(s.recv(1024).decode())

        #salva la manipolabilità
        if Mean_determinant!= -2147483648:
            dati.append(Mean_determinant)


        #inserisci nel file di testo
        with open("single_object.txt", "a") as File:
            File.write(f" oggetto di pick: Cube_0{pick}  \n base del manipolatore in y={base} \n manipolabilita'= {Mean_determinant} \n")


        if Mean_determinant== -2147483648:
            unusable.append(base)

            #inserisci nel file di testo
            with open(file_path3, "a") as File:
                File.write(f" \n se la base è in posizione: {base} non si può fare l'operazione \n") 




    #chiudi connessione
    s.close()

    min_ok= min(unusable)
    max_ok= max(unusable)

    #inserisci nel file di testo
    with open(file_path3, "a") as File:
        File.write(f" \n\n minimo: {min_ok} \n massimo:{max_ok} \n") 


if __name__ == "__main__":

    # Run the code
    main()
            






        

        











