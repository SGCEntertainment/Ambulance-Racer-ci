using UnityEngine.SceneManagement;
using UnityEngine;

namespace com.smallgamescompany.orientation
{
	public class Orientation : MonoBehaviour
	{
		private static Orientation instance;

		public ScreenOrientation game;

		private void OnEnable()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Destroy(gameObject);
			}

			DontDestroyOnLoad(gameObject);

			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
		}

		private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			int sceneCount = SceneManager.sceneCountInBuildSettings;

			ScreenOrientation screenOrientation = sceneCount > 1 && arg0.buildIndex == 0 ? ScreenOrientation.AutoRotation : game;

			Screen.orientation = screenOrientation;
		}
	}
}
