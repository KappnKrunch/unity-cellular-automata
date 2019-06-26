# Unity Cellular-Automata Genetic Simulation

The main goal of this project is to use a genetic algorithm to optimize cellular automata. Each cell has genetic traits, 
both geneotypes(unseen traits) and phenotypes (visible trais). Their main visible traits are their size, color, and pulse rate.
The stage starts out with a selection of cell with randomized traits. Each cell's goal is only to find enough food to create self similar cells.
Over the course of the simulation, the cells fit to the environment perpetuate and create copies of themselves that look the same and pulse at the same
time. Cells unable to find enough food, die off and are removed. The end result is a felxible system that allows for a variety of differently abled cells to
survive a randomized environment.

I also added various novel cells that function differently than the regular Basic Cell such as a hunter cell which eats other cells instead of food,
a cluster cell that stuns and attaches basic cells to itself in order to steal the food they produce, and a virus cell that attaches to other basic cells
like a parasite, as well as a simple character controller. These are there more to see what I could do while inhereting from the original cell class.

