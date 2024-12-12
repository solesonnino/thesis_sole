import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d.art3d import Poly3DCollection

class Cubo:
    def __init__(self, vertice, lunghezza, altezza, profondità):
        """
        Inizializza il cubo.
        :param vertice: Tuple (x, y, z) che rappresenta il vertice di riferimento
        :param lunghezza: Lunghezza del cubo lungo l'asse X
        :param altezza: Altezza del cubo lungo l'asse Y
        :param profondità: Profondità del cubo lungo l'asse Z
        """
        self.vertice = vertice
        self.lunghezza = lunghezza
        self.altezza = altezza
        self.profondità = profondità

    def calcola_vertici(self):
        """
        Calcola i vertici del cubo.
        :return: Lista di 8 tuple, ciascuna rappresentante un vertice
        """
        x, y, z = self.vertice
        l, h, p = self.lunghezza, self.altezza, self.profondità
        return [
            (x, y, z),
            (x + l, y, z),
            (x + l, y + h, z),
            (x, y + h, z),
            (x, y, z + p),
            (x + l, y, z + p),
            (x + l, y + h, z + p),
            (x, y + h, z + p)
        ]

    @staticmethod
    def disegna_cubi(cubi):
        """
        Disegna più cubi in uno spazio 3D utilizzando Matplotlib.
        :param cubi: Lista di oggetti Cubo.
        """
        fig = plt.figure()
        ax = fig.add_subplot(111, projection='3d')

        for cubo in cubi:
            vertici = cubo.calcola_vertici()

            # Definizione delle facce del cubo
            facce = [
                [vertici[0], vertici[1], vertici[2], vertici[3]],  # Base inferiore
                [vertici[4], vertici[5], vertici[6], vertici[7]],  # Base superiore
                [vertici[0], vertici[1], vertici[5], vertici[4]],  # Lato frontale
                [vertici[2], vertici[3], vertici[7], vertici[6]],  # Lato posteriore
                [vertici[0], vertici[3], vertici[7], vertici[4]],  # Lato sinistro
                [vertici[1], vertici[2], vertici[6], vertici[5]]   # Lato destro
            ]

            # Aggiunta delle facce al grafico
            ax.add_collection3d(Poly3DCollection(facce, facecolors='cyan', linewidths=1, edgecolors='r', alpha=0.6))

        # Impostazione delle etichette degli assi
        ax.set_xlabel('Asse X')
        ax.set_ylabel('Asse Y')
        ax.set_zlabel('Asse Z')

        # Imposta i limiti degli assi (calcolati in base a tutti i cubi)
        all_vertici = [vertice for cubo in cubi for vertice in cubo.calcola_vertici()]
        x_vals, y_vals, z_vals = zip(*all_vertici)
        ax.set_xlim([min(x_vals), max(x_vals)])
        ax.set_ylim([min(y_vals), max(y_vals)])
        ax.set_zlim([min(z_vals), max(z_vals)])

        plt.show()

# Esempio di utilizzo
if __name__ == "__main__":
    # Definizione di più cubi
    cubo1 = Cubo((0, 0, 0), 2, 3, 4)
    cubo2 = Cubo((3, 2, 1), 2, 2, 2)
    cubo3 = Cubo((6, 0, 0), 1, 1, 5)

    # Disegna tutti i cubi nella stessa scena
    Cubo.disegna_cubi([cubo1, cubo2, cubo3])
