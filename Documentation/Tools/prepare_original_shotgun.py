"""Extract the first CC0 Mossberg report without pitch shifting or tail damping.
Source: The Free Firearm Sound Library, N_26P.wav. See AudioSources.md.
Run with Python/numpy. Source downloads remain in ignored scratch/audio_sources.
"""
from pathlib import Path
import numpy as np
import wave
root=Path(__file__).resolve().parents[2]
source=next((root/'scratch/audio_sources').rglob('N_26P.wav'))
with wave.open(str(source)) as w:
 raw=w.readframes(w.getnframes());channels=w.getnchannels();rate=w.getframerate();width=w.getsampwidth()
assert width==3
b=np.frombuffer(raw,np.uint8).reshape(-1,3).astype(np.int32)
a=b[:,0]|(b[:,1]<<8)|(b[:,2]<<16)
a=((a^0x800000)-0x800000)/8388608.0
a=a.reshape(-1,channels).mean(axis=1)
onset=int(np.flatnonzero(np.abs(a)>.01)[0]);start=max(0,onset-int(.003*rate))
a=a[start:start+int(2.2*rate)]
# Only sample-rate conversion; original timing and pitch remain intact.
output_rate=44100
# Band-limit before downsampling to avoid folding ultrasonic report energy into audible hiss.
cutoff=.47*min(1,output_rate/rate)
n=np.arange(127)-63
kernel=2*cutoff*np.sinc(2*cutoff*n)*np.kaiser(127,8)
kernel/=kernel.sum()
a=np.convolve(a,kernel,mode='same')
a=np.interp(np.arange(round(len(a)*output_rate/rate))*rate/output_rate,np.arange(len(a)),a)
a-=a.mean();a*=.78/max(np.max(np.abs(a)),1e-9)
a[-353:]*=np.linspace(1,0,353)
output=root/'Assets/Resources/Audio/Weapons/Combat_Shotgun_Fire.wav'
with wave.open(str(output),'wb') as w:
 w.setparams((1,2,output_rate,0,'NONE','not compressed'));w.writeframes((a*32767).astype('<i2').tobytes())
print('Original-pitch shotgun report:',len(a)/output_rate,'seconds; peak:',float(np.max(np.abs(a))))
