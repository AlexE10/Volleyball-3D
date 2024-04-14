using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Test : NetworkBehaviour
{
    [Command]
    public void CmdSetAuthority(NetworkIdentity obj)
    {
        obj.AssignClientAuthority(connectionToClient);
    }
}
