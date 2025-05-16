import React, {useState, useEffect} from 'react'
export default function App() {

  // Containing the Array of First Table. Pwede siguro dito na rin ilagay ung mga iterations??
  const [allValues, setAllValues] = useState([]);
  
  // Total Variable and Total Constraints
  const  [variableCount, setVariableCount] = useState(0);
  const  [constraintCount, setConstraintCount] = useState(0);

  const headerPart = ["-","--","Cj"];
  let minimizeZ = [];
  const subjectsToConstraints = []; // contains 2d array

  for (let i = 1; i < variableCount+1; i++){
    minimizeZ.push(`z${i}x`); // OK
  }

  for (let i = 1; i < constraintCount + 1; i++){
      let array = [];
      
      for (let j = 1; j < variableCount+1; j++){
          array.push(`c${j}x`); // OK
      }
      array.push(`sign${i}`)
      array.push(`RHS${i}`)
      subjectsToConstraints.push(array);
  }

  // onChange pag nilagyan number input sa Min Z
  const changeMinimizeZvalue = (index, val) => {
    minimizeZ[index] = val
  }

  // onChange pag nilagyan number input sa Subject to Constraints
  const changeArrayValue = (r,c, val) => {
    subjectsToConstraints[r][c] = val
  }

  // Putting the RHS TO THE LEFT. OK
  const moveRHSToLeft = () => {
    for(let i = 0; i < constraintCount; i++){
      let lastElement = subjectsToConstraints[i].pop();
      subjectsToConstraints[i].unshift(lastElement); 
    }
  }
  

  // Putting the RHS TO THE LEFT. OK
  let signs = [];
  const gettingSigns = () => {
    for(let i = 0; i < constraintCount; i++){
      let lastElement = subjectsToConstraints[i].pop();
      signs.push(lastElement)
    }
  }

  // Adding Slack and Artificial Variables kaso di naka arrange.
  const slack_artificial = () => {
    let newMinZ = [];
    for(let i=0; i< minimizeZ.length; i++){
      headerPart.push(minimizeZ[i]);
      newMinZ.push(`x${i+1}`)
    }

    minimizeZ = [];
    minimizeZ = newMinZ;
    minimizeZ.unshift("Xb")
    minimizeZ.unshift("Cb")
    for(let i = 0; i < signs.length; i++){
      if(signs[i] == "<="){
        minimizeZ.push(`S${i+1}`)
        headerPart.push("0");
        for(let j = 0; j < constraintCount; j++){
          if(i==j){
            subjectsToConstraints[j].push(`1`)
            subjectsToConstraints[j].unshift(`0`)
            subjectsToConstraints[j].unshift(`S${i+1}`)
          }
          else{
            subjectsToConstraints[j].push(`0`)
          }
        }
      }
      if(signs[i] == "="){
        minimizeZ.push(`A${i+1}`)
        headerPart.push("-M");
        for(let j = 0; j < constraintCount; j++){
          if(i==j){
            subjectsToConstraints[j].push(`1`)
            subjectsToConstraints[j].unshift(`-M`)
            subjectsToConstraints[j].unshift(`A${i+1}`)
          }
          else{
            subjectsToConstraints[j].push(`0`)
          }
        }
      }
      if(signs[i] == ">="){
        minimizeZ.push(`S${i+1}`)
        minimizeZ.push(`A${i+1}`)
        headerPart.push("0");
        headerPart.push("-M");
        for(let j = 0; j < constraintCount; j++){
          if(i==j){
            subjectsToConstraints[j].push(`-1`)
            subjectsToConstraints[j].push(`1`)
            subjectsToConstraints[j].unshift(`-M`)
            subjectsToConstraints[j].unshift(`A${i+1}`)
          }
          else{
            subjectsToConstraints[j].push(`0`)
            subjectsToConstraints[j].push(`0`)
          }
        }
      }

    }
  }

  //Displaying and Fixing the array
  const changeInputValues = async (e) => {
    e.preventDefault();

    return new Promise(resolve => {
      moveRHSToLeft();
      gettingSigns();
      slack_artificial();
      resolve(); // Resolve the Promise when the synchronous operations are done
    });
  }

  //Inserting Min Z and headerPart to the allValues array.
  const insertingZvalues = async () => {
    return new Promise(resolve => {
      
      let headerPartClone = headerPart;
      let minZClone = minimizeZ;
      let stcClone = subjectsToConstraints;
      stcClone.unshift(minZClone);
      stcClone[0].unshift("B");
      stcClone.unshift(headerPartClone);
      allValues.push(stcClone);

      resolve();
    });
  }
  
  // Show Table pero sa console.
  const showTable = async (e) => {
    await changeInputValues(e);
    await insertingZvalues();

    console.log(allValues[0])
    solveFunction();
  }


  //Ok na to
  const [showSolution, setShowSolution] = useState(true)
  const dispSolution = () => setShowSolution(!showSolution) // Standby for now
  const [solution, setSolution] = useState();


  async function solveFunction() {
    let allTableAnswers = [];
    if(showSolution){
            allTableAnswers.push(
              <div className={`flex justify-center items-center max-h-[78vh] w-full border-1 p-5`}>
                <table className="bg-red-200 w-auto border ">
                  <tbody className='bg-white text-center text-xl'>
                    {(() => {
                      const rows = [];
                      for (let i = 0; i < constraintCount+2; i++) {
                        const cells = [];
                        for (let j = 0; j < allValues[0][0].length; j++) {
                          cells.push(<td key={j} className='border px-3'>{allValues[0][i][j]}</td>);
                        }
                        rows.push(<tr key={i - 1}>{cells}</tr>);
                      }
                      return rows;
                    })()}
                  </tbody>
                </table>
              </div>
            )

    }
    setSolution(allTableAnswers)
  }

  
  return (
    <div className='flex flex-col bg-amber-400 justify-center items-center'>
      <h1 className='text-2xl'>Minimization</h1>
      {/* Total Variable and Total Constraints Div */}
      <div className='flex gap-3 m-4'>
        <h1>Total Variables:</h1> <input type='number' className='bg-white border-2 w-15 text-center' onChange={(e)=>setVariableCount(parseInt(e.target.value))}/>
        <h1>Total Constraints:</h1> <input type='number' className='bg-white border-2 w-15 text-center' onChange={(e)=>setConstraintCount(parseInt(e.target.value))}/>
      </div>

      {/* MIN Z */}
      <div className="flex items-center m-4">
        <h1>Min: Z=</h1>

        {minimizeZ.map((val, index) => (
          <React.Fragment key={index}>
            <input
              type='number'
              id={val}
              placeholder={val}
              className='flex bg-white rounded w-14 border text-center'
              onChange={(e) => changeMinimizeZvalue(`${index}`, e.target.value)}
            />
            <h1>x<sub>{index + 1}</sub></h1> {/* Display x subscript index */}
            {index < minimizeZ.length - 1 && <div className='px-2'>+</div>} {/* For plus Sign */}
          </React.Fragment>
        ))}
      </div>

      
      <div className='bg-gray-200 flex flex-col'>
        <h1 className='text-xl border-2'>Subject to constraints</h1>

        <form onSubmit={showTable} className='border-2 p-2'>
          {subjectsToConstraints.map((constraintArray, constraintIndex) => (
            <div key={constraintIndex} className="flex flex-row space-x-1 items-center">
              {constraintArray.map((value, valueIndex) => {
                // Add plus signs only between the 'c1x', 'c2x', and 'c3x' terms
                const showPlus = valueIndex < variableCount - 1;
                const showX = valueIndex < variableCount;
                const signCol = variableCount; // SIGN COLUMN INDEX
                
                if(valueIndex != signCol){
                  return (
                    <React.Fragment key={valueIndex}>
                      <input
                        type='number'
                        id={`[${constraintIndex}][${valueIndex}]`}
                        className="flex p-2 bg-white border rounded w-14 ml-3 mb-3 mt-3 text-center"
                        placeholder={`[${constraintIndex}][${valueIndex}]`}
                        onChange={(e) => changeArrayValue(`${constraintIndex}`,`${valueIndex}`,e.target.value)}
                        required
                      />
                      {showX && <div className="text-black">x<sub>{valueIndex+1}</sub></div>}
                      {showPlus && <div className="text-black ml-1">+</div>}
                    </React.Fragment>
                  );
                } else if (valueIndex == signCol){
                  return (
                    <React.Fragment key={valueIndex}>
                      <select
                        id={`[${constraintIndex}][${valueIndex}]`}
                        name={`[${constraintIndex}][${valueIndex}]`}
                        className="flex text-center justify-center items-center bg-white border-1 rounded w-11 h-7 mx-2"
                        defaultValue=""
                        onChange={(e) => changeArrayValue(`${constraintIndex}`,`${valueIndex}`,e.target.value)}
                        required
                      >
                        <option value=""></option>
                        <option value="<=">&lt;=</option>
                        <option value=">=">&gt;=</option>
                        <option value="=">=</option>
                      </select>
                    </React.Fragment>
                  );
                }



              })}
            </div>
          ))}
          
          <div className='flex justify-center items-center'>
            <button type="submit" className='bg-sky-400 hover:bg-sky-700 cursor-pointer mt-10 w-50 h-20 border-2 mb-2'>Show Table Below</button>
          </div>
        </form>
      </div>
      {solution}
    </div>
  )


}

