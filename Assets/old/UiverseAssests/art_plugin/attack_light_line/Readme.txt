All effects consist of particle systems, models, scripts. For easy management, each effect is the main script "Effect Settings". 
He has common settings, such as "target", "collider radius", "move speed" and others. For a specific effect, some of the settings do not make sense (eg portal has no speed settings).

All parameters in all scripts are measured in meters and seconds. 

Script Parameters "Effect Settings": 

- ColliderRadius: allows you to change the distance to the collision. Example of a standard radius of the fireball visible 0.2m, hence "ColliderRadius" you need to install the same 0.2m.

- EffectRadius: in this version of the pack is not in use. Allows you to change some AOE effects range.

- Target: allows you set a target or a point in space, in the direction you want to move.
For example projectile shot in a straight line from the camera (First person viev):
	var go = new GameObject();
	go.transform.position = Camera.main.transform.forward + Camera.main.transform.position;
	effectSettings.Target = go; 

Or a shot towards the target:
effectSettings.Target = Enemy;

- MoveSpeed: allows you to adjust the speed of the projectile. 

- MoveDistance: allows you to adjust the range of the projectile. (Only if the effect is not homing). 

- IsHomingMove: allows you to specify the path of motion in a straight line or target homing. 

- IsVisible: allows you to enable / disable effects without motion. Such as flame or electric shield smoothly appear / disappear.

- DeactivateAfterCollision: allows you to specify whether to disable the main prefab projectile after collision. Need for use in the object pool. 
After collision, after the specified time, the object becomes inactive "effect.SetActive (false)". You can not remove the effect, and use it again, like this "efffect.SetActive (true)"; 
This avoids the overhead associated with instantiate / destroy.

- DeactivateTimeDelay: allows you to specify the delay time before switching off. 

As well, there are some events: 

- EffectDeactivated: event is called after the object has been turned off. For example, you can reset the status effect, and place it in the pull objects for further use.

- CollisionEnter: event is called after the collision, or if the projectile flew necessary distance in a straight line (moveDistance). In the event of a collision, you can get the point of impact "RaycastHit". This event will easily get the object with which the collision occurred. 
Example:

effectSettings.CollisionEnter += (n, e) =>
    {
      if (e.Hit.transform != null) Debug.Log("object name is " + e.Hit.transform.name);
      else Debug.Log("Projectile flew 'moveDistance'");
    };

To create the effect, enough to cause instance:
	
var effectInstance = Instantiate(effect, pos, new Quaternion()) as GameObject;

If you want to specify some options "EffectSettings":

var effectSettings = effectInstance.GetComponent<EffectSettings>();
effectSettings.Target = Target;


