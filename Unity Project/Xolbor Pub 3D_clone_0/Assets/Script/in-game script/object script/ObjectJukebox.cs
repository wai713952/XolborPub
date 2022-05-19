using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectJukebox : NetworkBehaviour
{
    //jukebox keeps playing the song for client in the bar zone


    public NetworkVariable<int> songIndexNetwork = new NetworkVariable<int>();
    public NetworkVariable<float> songTimeNetwork = new NetworkVariable<float>();
    public NetworkVariable<bool> isPlayingNextSong = new NetworkVariable<bool>();
    private float songTime;

    public List<AudioClip> songList = new List<AudioClip>();
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartSong()
    {
        audioSource.clip = songList[songIndexNetwork.Value];
        audioSource.time = songTimeNetwork.Value;
        audioSource.Play();
    }
    public void StartSongClient(ulong test)
    {
        StartSong();
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            songTime = songTimeNetwork.Value;
            SongTimeCheck();
            if (audioSource.isPlaying == false && isPlayingNextSong.Value == false)
            {
                isPlayingNextSong.Value = true;
                SongIndexCheck();
                ChangeSongServerRpc();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                TestChangeSongClientRpc();
            }
        }
    }
    private void SongTimeCheck()
    {
        songTimeNetwork.Value = audioSource.time;
    }
    private void SongIndexCheck()
    {
        //if the song's index reaches the last song; reset the count and start at first song
        if (songIndexNetwork.Value == songList.Count - 1)
        {
            songIndexNetwork.Value = 0;
        }
        //if it's not last song, keep counting
        else
        {
            songIndexNetwork.Value++;
        }
    }
    [ServerRpc(RequireOwnership = true)]
    private void ChangeSongServerRpc()
    {
        audioSource.time = 0;
        audioSource.clip = songList[songIndexNetwork.Value];
        audioSource.Play();
    }

    [ClientRpc]
    private void TestChangeSongClientRpc()
    {
        audioSource.time = 0;
        audioSource.clip = songList[1];
        audioSource.Play();
    }

}
