# Fixing Spatial-NonUniformity of different given planes

Different lighting conditions can lead to non-uniformities in an image plane,
one of them is Spatial non-uniformity that yields bright and dark regions in the target plane.

## Example image of non uniformed image

Spliting the image in 12 equal parts, we calculate the standard deviation of the pixel value in each part and construct
a new image, which can help us see the non uniformity clearer.

![nonUniform](https://github.com/KemerDev/Spatial-NonUniformityFix/blob/master/images/original.PNG "Our non uniformed image")
![nonUniform](https://github.com/KemerDev/Spatial-NonUniformityFix/blob/master/images/parts.PNG "Our non uniformed image")

## Example image of the above image's non uniformity translated in a white plane

![nonUniform](https://github.com/KemerDev/Spatial-NonUniformityFix/blob/master/images/whiteEq.PNG "Our non uniformed images")

![nonUniform](https://github.com/KemerDev/Spatial-NonUniformityFix/blob/master/images/target.PNG "Our non uniformed target")

We can instantly tell the problem that occurs which can ultimately lead to machine detection problems, of little fine details,
especially when our light setup, hits our target plane not only from above but from an angle causing shadows.

For fixing the problem we first crop both non uniformed image and it's translated white plane to our desired width and height,


![nonUniform](https://github.com/KemerDev/Spatial-NonUniformityFix/blob/master/images/finalProduct.PNG "fixed plane")
