using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectAbility : CharacterAbility
{
    public List<ParticleSystem> Effects;

    public void RequestPlay(int effectIndex)
    {
        Owner.PhotonView.RPC(nameof(Play), RpcTarget.All, effectIndex);
    }

    [PunRPC]
    private void Play(int effectIndex)
    {
        Effects[effectIndex].Play();
    }
}
