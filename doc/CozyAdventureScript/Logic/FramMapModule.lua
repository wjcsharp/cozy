methods = {
	'Requirement',
	'Exp',
	'Money',
}

local FightList = {0, 20, 40, 60, 80, 100, 200, 320, 440, 560, 680, 800, 1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400, 1450, 1500, 2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3500, 3720, 3940, 4160, 4380, 4600, 4820, 5040, 5260, 5480, 5700, 5920, 6140, 6360, 6580, 6800, 7020, 7240, 7460, 7680, 7900}
local ExpList = {1, 3, 5, 7, 9, 20, 32, 44, 56, 68, 80, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300,350, 372.5, 395, 417.5, 440, 462.5, 485, 507.5, 530, 552.5, 575, 597.5, 620, 642.5, 665, 687.5, 710, 732.5, 755, 777.5, 800}
local MoneyList = {1, 1, 1, 2, 2, 3, 4.4, 5.8, 7.2, 8.6, 15, 16.5, 18, 19.5, 21, 22.5, 24, 25.5, 27, 28.5, 30, 35, 36.5, 38, 39.5, 41, 42.5, 44, 45.5, 47, 48.5, 50, 60, 62, 64, 66, 68, 70, 72, 74, 76, 78, 80, 82, 84, 86, 88, 90, 92, 94, 96, 98, 100}

function Requirement(level)
	return FightList[level]
end

function Exp(level)
	return ExpList[level]
end

function Money(level)
	return MoneyList[level]
end
