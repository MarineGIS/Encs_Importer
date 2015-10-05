﻿#Encs_Importer


---- 소개
<br/>
 S-100 표준 기반의 다양한 분야의 데이터를 전자해도에 표출하기 위해 일관된 XML구조의 데이터 레코드 파일을 생성하는 클래스입니다.<br/>

기존 S-57 표준 기반 해도 데이터는 포맷 형식으로 ISO/IEC 8211국제 표준을 이용하였다. 그러나 다양한 분야의 데이터(S-10X)들을 화면에 표출하기위해서는 S-100 표준의 기본 구조 모델(input schema)을 기반으로 확장한 각 분야의 S-10X 구조 모델(Input schema)이 필요하다. 이에 구조 정의 시 ISO8211 파서 보다 여러 개발 환경에서 쉽게 접근 가능한 XML 파서를 사용하는 것이 S-100 표준을 적용한 데이터 레코드 생성 및 일관된 데이터 전송 처리에 용이할 것이다.

---- 특징 
<br/>
1. 기존 .000 전자해도 데이터를 이용한 S-100 표준 기반 데이터 레코드 생성.<br/>
2. 사용자가 필요로 하는 인코딩 타입을 추가 설정 및 적용 가능한 확장성.<br/>
3. 사용자가 필요로 하는 바인딩 타입 설정 및 적용 가능한 확장성.<br/>
4. 원하는 데이터 레코드 형식 설정 및 적용 가능한 유연한 출력 파일 생성.<br/>