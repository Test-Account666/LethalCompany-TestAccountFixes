using GameNetcodeStuff;
using TestAccountFixes.Core;
using UnityEngine;

namespace TestAccountFixes.Fixes.DogSound;

public class SprintChecker : MonoBehaviour {
    private const int SWITCH_THRESHOLD = 6;
    private const float SWITCH_TIME_FRAME = 1.0f;
    internal PlayerState currentState = PlayerState.WALK;
    private float _lastCheckTime;
    private Vector3 _lastPosition;

    private PlayerState _previousState = PlayerState.WALK;
    private int _switchCount;

    private static PlayerState GetPlayerState(PlayerControllerB playerControllerB) {
        var playerState = PlayerState.WALK;

        if (playerControllerB.IsOwner
                ? playerControllerB.isSprinting
                : playerControllerB.playerBodyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Sprinting"))
            playerState = PlayerState.SPRINT;

        if (playerControllerB.IsOwner
                ? playerControllerB.isCrouching
                : playerControllerB.playerBodyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("crouching"))
            playerState = PlayerState.CROUCH;

        return playerState;
    }

    private static void AlertDoggos(PlayerControllerB playerControllerB) {
        DogSoundFix.LogDebug($"[SilentSprint3] {playerControllerB.playerUsername} is alerting doggos!");

        var noiseIsInsideClosedShip = playerControllerB.isInHangarShipRoom && StartOfRound.Instance.hangarDoorsClosed;

        playerControllerB.PlayFootstepSound();

        RoundManager.Instance.PlayAudibleNoise(playerControllerB.transform.position, 13f, 0.6f,
                                               noiseIsInsideClosedShip: noiseIsInsideClosedShip, noiseID: 8);
    }


    internal void CheckForRapidStateChange(PlayerControllerB playerControllerB) {
        _previousState = currentState;
        currentState = GetPlayerState(playerControllerB);

        if (currentState == _previousState)
            return;

        var position = playerControllerB.transform.position;

        var isStandingStill = Vector3.Distance(position, _lastPosition) <= 0.001;

        _lastPosition = position;

        DogSoundFix.LogDebug(
            $"[SilentSprint3] {playerControllerB.playerUsername}: Switch from {_previousState} to {currentState} detected!",
            LogLevel.VERY_VERBOSE);

        _switchCount++;

        if (_switchCount >= SWITCH_THRESHOLD && !isStandingStill)
            AlertDoggos(playerControllerB);

        // Reset switch count if time frame elapsed
        if (Time.time - _lastCheckTime < SWITCH_TIME_FRAME)
            return;

        DogSoundFix.LogDebug($"[SilentSprint3] {playerControllerB.playerUsername}: Threshold expired!", LogLevel.VERY_VERBOSE);

        _switchCount = 0;
        _lastCheckTime = Time.time;
    }

    internal enum PlayerState {
        WALK,
        CROUCH,
        SPRINT,
    }
}