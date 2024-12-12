from py3dbp import Scene, Packer, Bin, Item

packer = Packer()

packer.add_bin(Bin('small-envelope', 12, 12, 12, 10))



packer.add_item(Item('50g [powder 1]', 6,6,6, 1))
packer.add_item(Item('50g [powder 2]', 6,6,6, 1))
packer.add_item(Item('50g [powder 3]', 6,6,6, 1))
packer.add_item(Item('250g [powder 4]', 6,6,6, 1))
packer.add_item(Item('250g [powder 5]', 6,6,6, 1))
packer.add_item(Item('250g [powder 6]', 6,6,6, 1))
packer.add_item(Item('250g [powder 7]', 6,6,6, 1))
packer.add_item(Item('250g [powder 8]', 6,6,6, 1))


packer.pack()

scene = Scene()

for b in packer.bins:
    print(":::::::::::", b.string())

    print("FITTED ITEMS:")
    for item in b.items:
        print("====> ", item.string())
        scene.add_object_to_scene(item)


    print("UNFITTED ITEMS:")
    for item in b.unfitted_items:
        print("====> ", item.string())

    print("***************************************************")
    print("***************************************************")
scene.show_scene()

