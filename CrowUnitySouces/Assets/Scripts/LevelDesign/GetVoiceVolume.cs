using UnityEngine;
using System.Collections;

public class GetVoiceVolume : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		// on veut get le groupe VX (pas le master)

//		// Get the master channel group
//		FMOD::ChannelGroup* master;
//		system->getMasterChannelGroup(&master);
//		
//		// Get the DSP unit that is at the head of the DSP chain of the master channel group
//		FMOD::DSP* masterHead;
//		master->getDSP(FMOD_CHANNELCONTROL_DSP_HEAD, &masterHead);
//		// enable output metering
//		masterHead->setMeteringEnabled(false, true);
//		
//		// Call this at regular intervals to fetch the output meter
//		FMOD_DSP_METERING_INFO outputmeter = {};
//		masterHead->getMeteringInfo(0, &outputmeter);
//		
//		// stereo on iOS
//		assert(outputmeter.numchannels == 2);
//		printf("Power over the last %d samples: left = %f, right = %f \n", outputmeter.numsamples, outputmeter.rmslevel[0], outputmeter.rmslevel[1]);

	}
}
