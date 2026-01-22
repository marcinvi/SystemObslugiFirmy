package com.example.ena.api

import java.time.OffsetDateTime

data class ReturnListItemDto(
    val id: Int,
    val referenceNumber: String,
    val waybill: String?,
    val buyerName: String,
    val productName: String?,
    val createdAt: OffsetDateTime,
    val statusAllegro: String?,
    val statusWewnetrzny: String?,
    val stanProduktu: String?,
    val decyzjaHandlowca: String?,
    val handlowiecId: Int?,
    val isManual: Boolean,
)

data class ReturnDetailsDto(
    val id: Int,
    val referenceNumber: String,
    val createdAt: OffsetDateTime,
    val statusWewnetrzny: String?,
    val statusAllegro: String?,
    val buyerLogin: String?,
    val buyerName: String,
    val buyerPhone: String?,
    val buyerAddress: String?,
    val buyerAddressRaw: String?,
    val buyerPhoneRaw: String?,
    val deliveryName: String?,
    val deliveryAddress: String?,
    val deliveryPhone: String?,
    val waybill: String?,
    val carrierName: String?,
    val productName: String?,
    val offerId: String?,
    val quantity: Int?,
    val reason: String?,
    val invoiceNumber: String?,
    val allegroAccountName: String?,
    val uwagiMagazynu: String?,
    val stanProduktuId: Int?,
    val stanProduktuName: String?,
    val przyjetyPrzezId: Int?,
    val przyjetyPrzezName: String?,
    val dataPrzyjecia: OffsetDateTime?,
    val decyzjaHandlowcaId: Int?,
    val decyzjaHandlowcaName: String?,
    val komentarzHandlowca: String?,
    val dataDecyzji: OffsetDateTime?,
    val isManual: Boolean,
    val allegroReturnId: String?,
    val orderId: String?,
)

data class ReturnWarehouseUpdateRequest(
    val stanProduktuId: Int,
    val uwagiMagazynu: String?,
    val dataPrzyjecia: OffsetDateTime,
    val przyjetyPrzezId: Int,
)

data class ReturnForwardToSalesRequest(
    val stanProduktuId: Int,
    val uwagiMagazynu: String?,
)

data class ReturnDecisionResponse(
    val returnId: Int,
    val statusWewnetrzny: String,
    val decyzjaHandlowca: String,
    val dataDecyzji: OffsetDateTime,
)

data class ReturnForwardToWarehouseRequest(
    val komentarz: String?,
)

data class ReturnSyncRequest(
    val accountId: Int?,
    val daysBack: Int?,
)

data class ReturnSyncResponse(
    val accountsProcessed: Int,
    val returnsFetched: Int,
    val returnsProcessed: Int,
    val startedAt: OffsetDateTime,
    val finishedAt: OffsetDateTime,
    val errors: List<String>,
)

data class ReturnManualCreateRequest(
    val numerListu: String,
    val produkt: String?,
    val przewoznik: String?,
    val stanProduktuId: Int,
    val uwagiMagazynu: String?,
    val buyerFullName: String,
    val buyerStreet: String?,
    val buyerZipCode: String?,
    val buyerCity: String?,
    val buyerPhone: String?,
    val wybraniHandlowcy: List<Int>,
)

data class ManualReturnRecipientDto(
    val id: Int,
    val nazwaWyswietlana: String,
)

data class ManualReturnMetaDto(
    val handlowcy: List<ManualReturnRecipientDto>,
    val produkty: List<String>,
    val przewoznicy: List<String>,
)

data class ReturnActionDto(
    val id: Int,
    val returnId: Int,
    val data: OffsetDateTime,
    val uzytkownik: String,
    val tresc: String,
)

data class ReturnActionCreateRequest(
    val tresc: String,
)

data class MessageCreateRequest(
    val nadawcaId: Int,
    val odbiorcaId: Int,
    val tytul: String?,
    val tresc: String,
    val dotyczyZwrotuId: Int?,
)

data class ReturnSummaryItemDto(
    val id: Int,
    val numerZwrotu: String,
    val produkt: String,
    val ktoPrzyjal: String,
    val ktoPodjalDecyzje: String,
    val jakaDecyzja: String,
    val uwagiMagazynu: String,
    val uwagiHandlowca: String,
    val status: String,
)

data class ReturnSummaryStatsDto(
    val total: Int,
    val doDecyzji: Int,
    val zakonczone: Int,
)

data class ReturnSummaryResponse(
    val items: List<ReturnSummaryItemDto>,
    val stats: ReturnSummaryStatsDto,
)

data class WarehouseSearchItemDto(
    val nrZgloszenia: String,
    val sn: String?,
    val model: String?,
    val klient: String?,
    val produktId: Int,
    val kategoria: String?,
)

data class WarehouseIntakeRequest(
    val nrZgloszenia: String,
    val model: String,
    val numerSeryjny: String?,
    val status: String,
    val lokalizacja: String,
    val czyDawca: Boolean,
    val uwagi: String?,
    val czesci: List<String>,
)

data class StatusDto(
    val id: Int,
    val nazwa: String,
    val typ: String,
)

data class ForwardToComplaintRequest(
    val returnId: Int,
    val powodKlienta: String?,
    val uwagiMagazynu: String?,
    val uwagiHandlowca: String?,
    val przekazal: String,
    val daneKlienta: ComplaintCustomerDto,
    val produkt: ComplaintProductDto,
)

data class ComplaintCustomerDto(
    val imie: String,
    val nazwisko: String,
    val email: String?,
    val telefon: String?,
    val adres: ComplaintAddressDto,
)

data class ComplaintAddressDto(
    val ulica: String?,
    val kod: String?,
    val miasto: String?,
)

data class ComplaintProductDto(
    val nazwa: String,
    val nrFaktury: String?,
    val nrSeryjny: String?,
)
