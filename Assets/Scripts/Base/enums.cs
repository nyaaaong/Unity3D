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
	MoveSpeed,
	MultiShot,
	Max
}

public enum Cheat_Type
{
	PowerUp,
	NoHit,
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

public enum Audio_Type
{
	Music,
	NeedBossSpawn,
	Max
}

public enum Bullet_Position
{
	Head,
	Tail,
	Left,
	Right,
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
	Menu,
	Continue
}

public enum Data_Type
{
	CharData,
	AbilityData,
	StageData,
	Audio,
}

public enum Boss_State
{
	None,
	NeedSpawn,
	Spawn,
	Clear,
	Max
}

public enum Particle_Type
{
	PlayerHit,
	MonsterHit,
	HitWall,
	Max
}

public enum CharClip_Type
{
	Player,
	Monster,
	Max
}