from place_visualize_obj import Scene, Packer, Bin, Item

packers=[]

packer1 = Packer()
packer2=Packer()
packers.append(packer1)
packers.append(packer2)
packer1.add_bin(Bin('small-envelope', 100, 100, 100, 20))
packer2.add_bin(Bin('Type2_box1', 25, 25, 25, 20))
packer2.add_bin(Bin('Type2_box2', 25, 25, 25, 20))
packer1.add_item(Item('Type1', 25,25,25, 1))

#packer2.add_item(Item('Type2', 25,25,25, 1))
packer2.add_item(Item('Type2_1', 25,25,25, 1))
packer2.add_item(Item('Type2_2', 25,25,25, 1))
#packer2.add_item(Item('Type2', 25,25,25, 1))
#packer2.add_item(Item('Type2', 25,25,25, 1))
#packer2.add_item(Item('Type2', 25,25,25, 1))
#packer2.add_item(Item('Type2', 25,25,25, 1))

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