using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace RotateElements
{
    class Iterator
    {
        public static ICollection<ElementId> GetElements(Document doc, Element elemForRotate, Connector conAxis)
        {
            ICollection<ElementId> listElements = new List<ElementId>();//список уникальный элементов трассировки

            List<ElementId> elemMoreTwoCon = new List<ElementId>();// список элементов у которых больше двух коннекторов

            List<int> countCon = new List<int>(); // лист показывающий сколько раз нужно пройти по элементу из "elemMoreTwoCon"

            ConnectorSet connectorSet = GetConnectorSet(elemForRotate); // получаю все коннектора элемента для вращения

            if (connectorSet == null) return listElements; // если их нет, то возвращаю пустой список метода
            ConnectorSetIterator csi = connectorSet.ForwardIterator(); // получаю итератор из списка коннекторов для прохода по каждому из них

            while (csi.MoveNext()) // проверяю можно ли идти вперёд по коннекторам и если можно то прохожу вперёд
                                   // (максимально возможная величина циклов = 20)
            {
                if (!(csi.Current is Connector connector)) continue; // проверяю являетмя ли текущий элемент коннектором и если
                                                                     // нет то пропускаю итерацию
                int count = GetConnectorSet(connector.Owner, out int countUnused).Size; // получаю общее количество коннекторов
                                                                                        // и неприсоединённые коннектора в текущем семействе

                if (count > 2 && !elemMoreTwoCon.Contains(connector.Owner.Id)) // отфильтровываю элементы у которых больше двух коннекторов 
                {
                    elemMoreTwoCon.Add(connector.Owner.Id); // добавляю в список по которому нужно будет потом пройти ещё n количество раз

                    countCon.Add(count - 2 - countUnused); //определяю сколько раз ещё нужно будет пройти по элементам
                }

                if (!listElements.Contains(connector.Owner.Id)) // добавляю уникальный элемент
                {
                    listElements.Add(connector.Owner.Id);
                }
                ConnectorSet conSet = connector.AllRefs; // получаю у коннектора ссылку на соседний коннектор с которым он соединён

                foreach (Connector elemCon in conSet) // прохожу по списку из одного или двух коннекторов-ссылок (у трубы 2 референса,
                                                      // у инстансев всегда один)
                {
                    if (!listElements.Contains(elemCon.Owner.Id) && GetConnectorSet(elemCon.Owner) != null
                                                                 && elemCon.Owner.Id != conAxis.Owner.Id)
                    {
                        csi = GetConnectorSet(elemCon.Owner).ForwardIterator(); // проеряю уникальность элемента и в случае если это следующий
                                                                                // элемент сети, то беру у него списко коннекторов
                    }

                }
            }
            for (int i = 0; i < elemMoreTwoCon.Count; i++) // цикл прохода по списку всех уэлементов у которых более двух коннекторов
            {
                ConnectorSet conSetGlobal = GetConnectorSet(doc.GetElement(elemMoreTwoCon[i])); // получаю спискок коннекторов у элемента "elemMoreTwoCon"

                foreach (Connector connector in conSetGlobal) // проходу по всем коннекторам элемента "elemMoreTwoCon"
                {
                    int x = 0; // счётчик захода в ответвление, чтобы отнять потом из элемента "elemMoreTwoCon"
                               // величину показывающую сколько раз по нему нужно пройти

                    ConnectorSet conSet = connector.AllRefs; // поиск референсов у коннекторам элемента "elemMoreTwoCon"
                    ConnectorSetIterator csi_3 = conSet.ForwardIterator();// получаю итератор из списка референсов для прохода по каждому из них

                    while (csi_3.MoveNext()) // проходчик по референсам, максимум 2 элемента списка
                    {
                        if (!(csi_3.Current is Connector elemCon)) continue;// проверяю являетмя ли текущий элемент коннектором и если
                                                                            // нет то пропускаю итерацию
                        ConnectorSet cs = GetConnectorSet(elemCon.Owner, out int countUnused);// получаю общее количество коннекторов
                                                                                              // и неприсоединённые коннектора в текущем семействе
                        int count = cs.Size;

                        if (count > 2 && !elemMoreTwoCon.Contains(elemCon.Owner.Id)) // если на пути прохода итератора встерчается элемент с кол-вом коннекторов >2
                                                                                     //и мы по нему ещё ни разу не проходили до добавляем его в список и даём
                                                                                     //номер-количество проходов 
                        {
                            elemMoreTwoCon.Add(elemCon.Owner.Id);
                            countCon.Add(count - 2 - countUnused);

                        }

                        ConnectorSet conSet_2 = elemCon.AllRefs;

                        foreach (Connector item in conSet_2)
                        {
                            if (item.Owner.Id == elemMoreTwoCon[i])
                            {
                                if (!listElements.Contains(elemCon.Owner.Id) && GetConnectorSet(elemCon.Owner) != null
                                                                             && elemCon.Owner.Id != elemMoreTwoCon[i] && elemCon.Owner.Id != conAxis.Owner.Id)
                                {
                                    listElements.Add(elemCon.Owner.Id);
                                    if (x == 0)
                                    {
                                        countCon[i]--;
                                        x++;
                                    }
                                    csi_3 = GetConnectorSet(elemCon.Owner).ForwardIterator();
                                }

                            }
                            else
                            {
                                        
                                if (!listElements.Contains(item.Owner.Id) && GetConnectorSet(item.Owner) != null
                                                                          && item.Owner.Id != elemMoreTwoCon[i] && elemCon.Owner.Id != conAxis.Owner.Id)
                                {
                                    listElements.Add(item.Owner.Id);
                                    if (x == 0)
                                    {
                                        countCon[i]--;
                                        x++;
                                    }
                                    csi_3 = GetConnectorSet(item.Owner).ForwardIterator();
                                }
                                        

                            }
                        }
                    }



                        
                }
            }

            return listElements;
        }

        public static ConnectorSet GetConnectorSet(Element element, out int countUnused)
        {
            if (element is FamilyInstance family)
            {
                ConnectorManager list = family.MEPModel.ConnectorManager;
                countUnused = list.UnusedConnectors.Size;
                ConnectorSet connectorSet = list.Connectors;
                return connectorSet;
            }

            if (element is MEPCurve curve)
            {
                ConnectorManager list = curve.ConnectorManager;
                countUnused = list.UnusedConnectors.Size;
                ConnectorSet connectorSet = list.Connectors;
                return connectorSet;
            }

            countUnused = 0;
            return null;
        }
        public static ConnectorSet GetConnectorSet(Element element)
        {
            if (element is FamilyInstance family)
            {
                ConnectorManager list = family.MEPModel.ConnectorManager;
                ConnectorSet connectorSet = list.Connectors;
                return connectorSet;
            }

            if (element is MEPCurve curve)
            {
                ConnectorManager list = curve.ConnectorManager;
                ConnectorSet connectorSet = list.Connectors;
                return connectorSet;
            }

            return null;
        }


    }
}
