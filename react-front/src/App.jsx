import { CloseContext } from 'contexts/сloseContext';
import { useClose } from 'hooks/useClose';
import Router from 'routes';



function App() {

  const { itms, add, remove, closeAll } = useClose()
 


  return (
    <CloseContext.Provider value={{ add, remove, closeAll }}>
      <main onClick={() => closeAll()} onContextMenu={(evt) => { evt.preventDefault(); closeAll() }}>
        <Router />
      </main>

    </CloseContext.Provider>

  )
}

export default App
