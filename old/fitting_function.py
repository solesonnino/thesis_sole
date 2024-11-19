# Definizione della funzione di fitting (tramite comunicazione con simulatore)

# Modifica
import socket
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
        s.sendall(f"{x}".encode())
        
        # Riceve la misura di fitting dal client
        data = s.recv(1024)
        return float(data.decode())
        
    client.close() # close the socket once the condition for the while loop is not satisfied anymore
