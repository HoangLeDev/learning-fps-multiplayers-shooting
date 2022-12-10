/* /////PlayerShooting.cs:
 * 1. [Done] Use SpawnPool asset to Spawn bullet impact for optimize
 * 2. Change weapon overheat to out of Ammo
 * 3. Write common script for Effect Counter
*/

/* /////PlayerController.cs:
 * 1. While crunching under the block, user still can release the crunch and stuck into the block => need to fix
 * 2. Add Animation while Switch gun or Add Text Show When Changing gun 
*/

/* /////Launcher.cs
 * 1. Script is now Init by Start(), need to create a InitLauncher() method and call in GameManager
 * => Make GameManager to be DontDestroyOnLoad by Create InitScene => Call All Init Controllers in this class
 *
 * 2. [Done] Create Constant script to store quote, notification, message as String Constant
 * 
 * 3. <Optional/Unnecessary> Launcher script is currently a lot of SetActive Panel (true/false)
 * => Create a Panel System to handle all main panel in an Array
 */

