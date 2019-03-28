using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour {
    void Start() {
        string path = CardMetadata.FILE_PATH_BASE;
        if (!System.IO.Directory.Exists(path)) {
            System.IO.Directory.CreateDirectory(path);
        }

        //This is a safeguard for if somebody closes the test scene without hitting end turn
        if(System.IO.File.Exists(CardMetadata.TEST_SCENE_FILE)) {
            System.IO.File.Delete(CardMetadata.TEST_SCENE_FILE);
        }
    }
}
