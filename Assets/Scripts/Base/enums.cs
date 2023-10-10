public enum Char_Type
{
	Player,
	Slime1, // 총 안쏨
	Slime2, // 총 쏨
	Boss1, // 총 안쏨
	Boss2, // 총 쏨
	Max
}

public enum Anim_Type
{
	Idle,
	Move,
	Attack,
	Death,
	Max
}

public enum AbilityRect_Position
{
	Left,
	Center,
	Right,
	Max
}

public enum Ability_Type
{
	Damage,
	FireRate,
	Heal,
	Speed,
	MultiShot,
	Max
}

public enum Cheat_Type
{
	PowerUp,
	NoHit,
	StageClear,
	Death,
	AddExp,
	Max
}

public enum Audio_Char
{
	Attack,
	Death,
	Max
}

public enum Audio_Effect
{
	BulletHit,
	AbilityUI,
	Max
}

public enum Audio_Type
{
	Character,
	Music,
	Max
}

public enum Bullet_Position
{
	Head,
	Tail,
	Max
}

public enum Bullet_Type
{
	Player,
	Monster,
	Max
}

public enum Popup_Type
{
	Wave,
	StageClear
}

public enum NavMask_Position
{
	Left,
	Top,
	Right,
	Bottom,
	Max
}

public enum Menu_Type
{
	Option,
	Continue
}

public enum Data_Type
{
	CharData,
	AbilityData,
	StageData,
	Audio,
}