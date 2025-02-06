
import socket
import numpy as np
from place_visualize_obj import Scene, Packer, Bin, Item
import os
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
import random
import statistics
import math

def main():

    Ndat=10
    i=0
    dati = []
    upper_bound=500
    lower_bound=-500
    v_max= 70 #mm/s
    a_max= 90 #mm/s^2

    file_path1="dataset_time.txt"
    file_path2= "media_varianza_time.txt"
     #svuoto il file se esiste
    with open(file_path1, 'w') as f:
        pass

        #svuoto il file se esiste
    with open(file_path2, 'w') as f:
        pass


    #loop for the number of samples
    for i in range(0, Ndat):
        start_pos= random.uniform(lower_bound,upper_bound)
        end_pos= random.uniform(lower_bound,upper_bound)

        #compute the time needed 
        d = abs(start_pos-end_pos)
        d_acc= pow(v_max,2)/a_max
        d_cost = d-2*d_acc
        if (d_cost <=0) : #triangular velocity profile
            t= 2*math.sqrt(d/a_max)
        else:
            t=2*(v_max/a_max)+d_cost/v_max

        dati.append(t)

        #inserisci nel file di testo
        with open(file_path1, "a") as File:
            File.write(f" dato: {i} \n partenza: {start_pos} \n arrivo: {end_pos} \n tempo: {t} \n\n")


    media= statistics.mean(dati)
    varianza= statistics.variance(dati,media)

    with open(file_path2, "a") as File:
        File.write(f"media tempo: {media} \n varianza tempo: {varianza}")

if __name__ == "__main__":

    # Run the code
    main()
            





        
        