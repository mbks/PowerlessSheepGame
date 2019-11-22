extends "res://Character.gd"

var textures = ['mage_f.png', 'mage_m.png',
				'healer_f.png', 'healer_m.png',
				'townfolk1_f.png', 'townfolk1_m.png']

func _ready():
	randomize()
	var texture = textures[randi() % textures.size()]
	texture = load("res://art/rpgsprites1/%s" % texture)
	$Sprite.texture = texture
	facing = moves.keys()[randi() % 4]

func _process(delta):
	if can_move:
		if not move(facing) or randi() % 10 > 5:
			facing = moves.keys()[randi() % 4]
