using Assets.Code.Tools.Base;
using UnityEngine;
using YG;

namespace Assets.Code.Core
{
    public class CameraOrientationHandler : MonoBehaviour
    {
        [SerializeField] private Vector3 _portraitOffset = new(0, 10, -5);
        [SerializeField] private Vector3 _landscapeOffset = new(0, 5, -10);

        private Follower _follower;
        private ScreenOrientation _currentOrientation;

        private bool IsPortrait => _currentOrientation == ScreenOrientation.Portrait || _currentOrientation == ScreenOrientation.PortraitUpsideDown;

        private void Start()
        {
            _follower = Camera.main.GetComponentOrThrow<Follower>();
            _currentOrientation = Screen.orientation;

            if (YG2.envir.isMobile || YG2.envir.isTablet)
            {
                ApplyCurrentOrientation();
            }
        }

        private void Update()
        {
            if (Screen.orientation != _currentOrientation)
            {
                _currentOrientation = Screen.orientation;
                ApplyCurrentOrientation();
            }
        }

        private void ApplyCurrentOrientation()
        {
            Vector3 targetOffset = IsPortrait ? _portraitOffset : _landscapeOffset;
            _follower.SetOffset(targetOffset);
        }
    }
}