
using UnityEngine;


public class MGBlendTable
{
	int mTableSzX, mTableSzY;
	int mnToBlend;
	int mTableSzXY;		// layer sz
	int mTotalSz;			// all layers
	float[] mpTable;	// the dither tables
	Random mRandom;		// maxsdk pseudo-random number generator

	int LayerIndex(int nImage)
	{
		return nImage * mTableSzXY;
	}
	
	// x,y index within a layer....add to layerIndex
	int PixelIndex(int x, int y)
	{
		return y * mTableSzX + x;
	}

	// 0,y index within a layer....add to layerIndex
	int RowIndex(int y)
	{
		return y * mTableSzX;
	}

	// these functions wrap a coordinate around the tile sz x or y, 
	// return index on a tile given a general x,y
	int TileX(int x)
	{
		return x % mTableSzX;
	}

	int TileY(int y)
	{
		return y % mTableSzY;
	}
	
	// user level index call, wraps to tile...
	int Index(int nImage, int x, int y)
	{
		//wrap to the tile
		int x0 = TileX(x);	
		int y0 = TileY(y);
		int indx = LayerIndex(nImage) + PixelIndex(x0, y0);
		return indx;
	}

	// get the table pointer, do your own indexing....
	float[] GetTable()
	{
		return mpTable;
	}

	// slow, but they do it all
	float GetWeight(int nImage, int x, int y)
	{
		int indx = Index(nImage, x, y);
		return mpTable[indx];
	}

	void SetWeight(int nImage, int x, int y, float w)
	{
		int indx = Index(nImage, x, y);
		mpTable[indx] = w;
	}

	// returns tile size of the table
	void GetTileSz(out int xSz, out int ySz)
	{
		xSz = mTableSzX;
		ySz = mTableSzY;
	}

	float dummyNoise()
	{
		return 0.5f;
	}

	float RandomNoise()
	{
		return Random.value;
	}

	public MGBlendTable(int tableX, int tableY, int nToBlend, float ditherAmt, bool normalizeTable)
	{
		// start w/ the size of each image layer of the table
		mTableSzX = (tableX <= 0) ? 1 : tableX;
		mTableSzY = (tableY <= 0) ? 1 : tableY;;
		mTableSzXY = mTableSzX * mTableSzY;

		// then get the total table size
		mnToBlend = nToBlend;
		mTotalSz = mTableSzXY * mnToBlend;
		//DbgAssert( mTotalSz > 0 );

		// alloc the 3D table, nImages of tableX x tableY layers
		// we have to manage the indices ourselves
		mpTable = new float[mTotalSz];

		// the initial weight for each image is 1/nToBlend; 10 images, each 1/10th
		float initWeight = 1.0f / ((mnToBlend == 0) ? 1.0f : (float)mnToBlend);

		// noise function is 0..1, so noiseAmplitude scales the 0..1 range
		// such that at ditherAmt == 1.0, the ampitude of the noise is the
		// same as the initial weight
		float noiseAmplitude = initWeight * ditherAmt;

		// fill in the table, just go thru the table linearly, not xy specific
		// worry about normalizing things later...
		for ( int i = 0; i < mTotalSz; ++i )
		{
			// compute weight for this sample. note we shift the 0..1 noise
			// to be -0.5 to +0.5 prior to scaling
			float noiseWeight = noiseAmplitude * (RandomNoise() - 0.5f);
			float weight = initWeight + noiseWeight;

			// i think it's possible things cd be out of range, since ditherAmt
			// is not bounded
			mpTable[i] = Mathf.Clamp01(weight);
			//Debug.Log("w " + i + " " + mpTable[i]);
		}

		// see if we need to normalize the table
		if ( normalizeTable )
		{
			// yes , normalize it
			// for each pixel in the table....
			for ( int y = 0; y < mTableSzY; ++y )
			{
				for ( int x = 0; x < mTableSzX; ++x )
				{
					// useful to pre compute this, since it's 
					// fixed as we scan the layers 
					int pixelIndex = PixelIndex(x, y);

					// find the sum of all the layers thru one pixel
					float sum = 0.0f;
					for ( int n = 0; n < mnToBlend; ++n )
					{
						// compute final index by adding in layer
						int index = LayerIndex(n) + pixelIndex;
						// add this layer to the sum
						sum += mpTable[index];
					}

					// now normalize...make the sum == 1.0
					float norm = 1.0f / ((sum == 0.0f) ? 1.0f : sum);

					for ( int n = 0; n < mnToBlend; ++n )
					{
						int index = LayerIndex(n) + pixelIndex;
						// scale this layer by the norm
						mpTable[index] *= norm;
					}
				}
			}
		}
	}

	public void BlendImages(Color[,] pDstBM, Color[,] pSrcBM, int width, int height, int nImage)
	{
		// index at (nImage,0,0)
		int layerIndex = LayerIndex(nImage);

		// for each line of the image...
		for ( int nLine = 0; nLine < height; ++nLine )
		{
			int rowIndex = RowIndex(TileY(nLine)) + layerIndex;

			//int index = nLine * width;

			for ( int nPix = 0; nPix < width; ++nPix )
			{
				float w = mpTable[rowIndex + TileX(nPix)];
				pDstBM[nPix, nLine] += pSrcBM[nPix, nLine] * w;	//pSrcBM[index + nPix];

				Mathf.Clamp01(pDstBM[nPix, nLine].r);
				Mathf.Clamp01(pDstBM[nPix, nLine].g);
				Mathf.Clamp01(pDstBM[nPix, nLine].b);
			}
		}
	}
}

[ExecuteInEditMode]
public class DOFCamera : MonoBehaviour
{
	public Camera	srcCamera;
	public float sampleRadius = 4.0f;
	public float alpha;
	public float focalDistance = 8.0f;

	Vector3 tpos;

	void Update()
	{
		if ( srcCamera != null )
		{
			float theta = alpha * Mathf.PI * 2.0f;

			float radius = sampleRadius;

			float uOffset = radius * Mathf.Cos(theta);
			float vOffset = radius * Mathf.Sin(theta);

			Vector3 newCameraLocation = new Vector3(uOffset, vOffset, 0.0f);
			Vector3 initialTargetLocation = srcCamera.transform.forward * focalDistance;	//new Vector3(0.0f, 0.0f, -focalDistance);

			tpos = initialTargetLocation + srcCamera.transform.position;	//srcCamera.transform.TransformPoint(initialTargetLocation);
			transform.position = srcCamera.transform.TransformPoint(newCameraLocation);
			transform.LookAt(tpos);
		}
	}

	public void OnDrawGizmos()
	{
		if ( srcCamera != null )
		{
			Gizmos.DrawLine(transform.position, tpos);
		}
	}
}