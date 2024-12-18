from place_visualize_obj import Scene, Packer, Bin, Item

packer = Packer()
packer.add_bin(Bin('small-envelope', 78, 78, 78, 10))
packer.add_item(Item('50g [powder 1]', 26,26,26, 1))
packer.add_item(Item('50g [powder 2]', 26,26,26, 1))
packer.add_item(Item('50g [powder 3]', 26,26,26, 1))
packer.pack()
scene = Scene()

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
    
    print("UNFITTED ITEMS:")
    for item in b.unfitted_items:
        print("====> ", item.string())

    print("***************************************************")
    print("***************************************************")
scene.show_scene()
print(place_points)