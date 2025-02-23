from place_visualize_obj import Scene, Packer, Bin, Item
import time

start = time.time()
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

items=[] 


for packer in packers:
    packer.pack()
    for b in packer.bins:
            scene = Scene()
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
            scene.show_scene()

    end = time.time()
    difference = end-start

    print(f"{end} \n {start} \n {difference}")